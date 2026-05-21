import { useEffect } from 'react'
import { useEditor, EditorContent } from '@tiptap/react'
import StarterKit from '@tiptap/starter-kit'
import Image from '@tiptap/extension-image'
import Link from '@tiptap/extension-link'
import Placeholder from '@tiptap/extension-placeholder'
import { cn } from '@/shared/lib/utils'
import { Toolbar } from './Toolbar'
import { richTextEditorClass } from './styles'

export interface RichTextEditorProps {
  value: string
  onChange: (html: string) => void
  onBlur?: () => void
  placeholder?: string
  disabled?: boolean
  onImageUpload?: (file: File) => Promise<string>
  className?: string
}

export function RichTextEditor({
  value,
  onChange,
  onBlur,
  placeholder,
  disabled,
  onImageUpload,
  className,
}: RichTextEditorProps) {
  const editor = useEditor({
    extensions: [
      StarterKit,
      Image.configure({ inline: false, allowBase64: false }),
      Link.configure({ openOnClick: false, autolink: true }),
      Placeholder.configure({ placeholder: placeholder ?? '' }),
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
