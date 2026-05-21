/**
 * Tailwind classes shared between RichTextEditor (ProseMirror div) and RichTextContent (viewer).
 * Uses arbitrary child-selector variants so no custom CSS is needed.
 */
export const richTextClass = [
  // Base
  'text-sm text-foreground leading-relaxed',
  // Headings
  '[&_h1]:text-xl [&_h1]:font-bold [&_h1]:tracking-tight [&_h1]:mt-4 [&_h1]:mb-2 [&_h1]:text-foreground',
  '[&_h2]:text-base [&_h2]:font-semibold [&_h2]:tracking-tight [&_h2]:mt-3 [&_h2]:mb-1.5 [&_h2]:text-foreground',
  // Paragraphs
  '[&_p]:mb-1.5 [&_p:last-child]:mb-0',
  // Inline
  '[&_strong]:font-semibold [&_strong]:text-foreground',
  '[&_em]:italic',
  '[&_s]:line-through [&_s]:opacity-50',
  // Code
  '[&_code]:font-mono [&_code]:text-xs [&_code]:bg-muted [&_code]:text-foreground [&_code]:px-1.5 [&_code]:py-0.5 [&_code]:rounded',
  // Lists
  '[&_ul]:list-disc [&_ul]:pl-5 [&_ul]:my-1.5',
  '[&_ol]:list-decimal [&_ol]:pl-5 [&_ol]:my-1.5',
  '[&_li]:mb-0.5 [&_li]:text-foreground',
  // Links
  '[&_a]:text-primary [&_a]:underline [&_a]:underline-offset-2 [&_a]:decoration-primary/50 [&_a:hover]:decoration-primary',
  // Images
  '[&_img]:rounded-md [&_img]:max-w-full [&_img]:my-3 [&_img]:border [&_img]:border-border',
  // Blockquote
  '[&_blockquote]:border-l-2 [&_blockquote]:border-border [&_blockquote]:pl-3 [&_blockquote]:text-muted-foreground [&_blockquote]:my-2 [&_blockquote]:italic',
].join(' ')

/** Extra classes only needed inside the editable ProseMirror div */
export const richTextEditorClass = [
  richTextClass,
  'outline-none min-h-[120px] px-3 py-2.5',
  // Tiptap placeholder via arbitrary property — no custom CSS needed
  '[&_.is-editor-empty:first-child::before]:[content:attr(data-placeholder)]',
  '[&_.is-editor-empty:first-child::before]:text-muted-foreground',
  '[&_.is-editor-empty:first-child::before]:pointer-events-none',
  '[&_.is-editor-empty:first-child::before]:[float:left]',
  '[&_.is-editor-empty:first-child::before]:[height:0]',
].join(' ')
