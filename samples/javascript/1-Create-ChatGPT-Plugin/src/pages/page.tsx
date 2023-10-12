import Image from 'next/image'
import styles from './page.module.css'

export default function Home() {
  return (
    <main className={styles.main}> 
      <h1 className={styles.title}>
        Welcome to Open AI next js starter plugin!
      </h1>
    </main>
  )
}
