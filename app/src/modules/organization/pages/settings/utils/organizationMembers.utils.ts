export const getMemberInitials = (name: string) => {
  const tokens = name
    .split(' ')
    .map((token) => token.trim())
    .filter(Boolean)

  if (tokens.length === 0) {
    return '??'
  }

  if (tokens.length === 1) {
    return tokens[0].slice(0, 2).toUpperCase()
  }

  return `${tokens[0][0]}${tokens[1][0]}`.toUpperCase()
}

export const getRoleAccentClassName = (index: number) => {
  const palette = [
    'bg-blue-500',
    'bg-emerald-500',
    'bg-violet-500',
    'bg-cyan-500',
    'bg-amber-500',
    'bg-rose-500',
  ]

  return palette[index % palette.length]
}
