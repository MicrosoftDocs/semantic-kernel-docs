import './globals.css'
import type { Metadata } from 'next' 

export const metadata: Metadata = {
  title: "Math Plugin - ChatGPT Plugin to display result on numbers",
  description:
    "Used to perform math operations (i.e., add, subtract, multiple, divide)",
  authors: { name: "anilgaya"},
  metadataBase: new URL("https://example.com"),
  themeColor: "#FFF",
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en"> 
      <body>{children}</body>
    </html>
  )
}
