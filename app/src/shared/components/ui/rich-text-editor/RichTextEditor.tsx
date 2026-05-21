import { useEffect, useRef } from 'react'
import { useEditor, EditorContent, ReactRenderer } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Image from '@tiptap/extension-image'
import Link from '@tiptap/extension-link'
import Placeholder from '@tiptap/extension-placeholder'
import Mention from '@tiptap/extension-mention'
import type { SuggestionProps, SuggestionKeyDownProps } from '@tiptap/suggestion'
import { cn } from '@/shared/lib/utils'
import { Toolbar } from './Toolbar'
import { richTextEditorClass } from './styles'
import { MentionList, type MentionListHandle, type MentionUser } from './MentionList'

export interface RichTextEditorProps {
  value: string
  onChange: (html: string) => void
  onBlur?: () => void
  placeholder?: string
  disabled?: boolean
  onImageUpload?: (file: File) => Promise<string>
  mentionUsers?: MentionUser[]
  className?: string
}

export function RichTextEditor({
  value,
  onChange,
  onBlur,
  placeholder,
  disabled,
  onImageUpload,
  mentionUsers,
  className,
}: RichTextEditorProps) {
  const mentionUsersRef = useRef<MentionUser[]>(mentionUsers ?? [])
  useEffect(() => { mentionUsersRef.current = mentionUsers ?? [] }, [mentionUsers])

  const editor = useEditor({
    extensions: [
      StarterKit,
      Image.configure({ inline: false, allowBase64: false }),
      Link.configure({ openOnClick: false, autolink: true }),
      Placeholder.configure({ placeholder: placeholder ?? '' }),
      Mention.configure({
        HTMLAttributes: { class: '' },
        renderHTML({ node }) {
          return [
            'span',
            {
              'data-type': 'mention',
              'data-id': node.attrs.id,
              'data-label': node.attrs.label,
            },
            `@${node.attrs.label}`,
          ]
        },
        suggestion: {
          items: ({ query }) =>
            mentionUsersRef.current
              .filter((u) => u.name.toLowerCase().includes(query.toLowerCase()))
              .slice(0, 6),

          render: () => {
            let renderer: ReactRenderer<MentionListHandle> | null = null
            let wrapper: HTMLDivElement | null = null

            return {
              onStart: (props: SuggestionProps) => {
                wrapper = document.createElement('div')
                wrapper.style.position = 'absolute'
                wrapper.style.zIndex = '9999'
                document.body.appendChild(wrapper)

                renderer = new ReactRenderer(MentionList, {
                  props,
                  editor: props.editor,
                })

                wrapper.appendChild(renderer.element)
                positionWrapper(wrapper, props)
              },

              onUpdate: (props: SuggestionProps) => {
                renderer?.updateProps(props)
                if (wrapper) positionWrapper(wrapper, props)
              },

              onKeyDown: (props: SuggestionKeyDownProps) => {
                if (props.event.key === 'Escape') {
                  cleanup()
                  return true
                }
                return renderer?.ref?.onKeyDown(props) ?? false
              },

              onExit: () => cleanup(),
            }

            function positionWrapper(el: HTMLDivElement, props: SuggestionProps) {
              const rect = props.clientRect?.()
              if (!rect) return
              el.style.left = `${rect.left + window.scrollX}px`
              el.style.top = `${rect.bottom + window.scrollY + 4}px`
            }

            function cleanup() {
              renderer?.destroy()
              wrapper?.remove()
              renderer = null
              wrapper = null
            }
          },
        },
      }),
    ],
    content: value,
    editable: !disabled,
    onUpdate: ({ editor: e }) => {
      const html = e.getHTML()
      onChange(html === '<p></p>' ? '' : html)
    },
    onBlur: () => onBlur?.(),
    editorProps: {
      attributes: { class: richTextEditorClass },
    },
  })

  useEffect(() => {
    if (!editor) return
    if (editor.isFocused) return
    const current = editor.getHTML()
    const normalised = current === '<p></p>' ? '' : current
    if (normalised !== value) {
      editor.commands.setContent(value || '', false)
    }
  }, [editor, value])

  useEffect(() => {
    if (!editor) return
    editor.setEditable(!disabled)
  }, [editor, disabled])

  if (!editor) return null

  return (
    <div
      className={cn(
        'rounded-md border border-input bg-background shadow-sm',
        'focus-within:outline-none focus-within:ring-1 focus-within:ring-ring',
        disabled && 'cursor-not-allowed opacity-60',
        className
      )}
    >
      <Toolbar editor={editor} onImageUpload={disabled ? undefined : onImageUpload} />
      <EditorContent editor={editor} />
    </div>
  )
}
