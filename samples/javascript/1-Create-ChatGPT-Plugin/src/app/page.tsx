import Image from 'next/image'
import styles from './page.module.css'   

export default function Home() { 

  return (<>
      <main className={styles.main}>
        <div> 
          Next Js OpenAi plugin, With Automated process to generate the swagger doc
          <br/>
          <br/>
          <a href='/.well-known/ai-plugin.json'> OpenAi Plugin Manifest File </a>
          <br/>
          <br/>
          <a href='/swagger.json'> Open-Api Spec File </a> 
        </div> 
      </main> 
  </>
  )
}
