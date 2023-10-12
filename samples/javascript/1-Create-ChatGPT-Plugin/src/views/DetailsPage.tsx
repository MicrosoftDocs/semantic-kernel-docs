"use client";
import { useSearchParams } from 'next/navigation';

export function DetailsPage() {  
  const searchParams = useSearchParams(); 
  const urlParams  = new URLSearchParams(searchParams?.toString() as string);
  let paramsObject = Object.fromEntries(urlParams.entries()); 

  return ( 
    <div>
    <main>
        <a
          target="_blank"
          rel="noreferrer" 
        >
          Deploy to you domain
        </a>
        <h1>Math Plugin {paramsObject?.method} Result</h1>
        <p className="description">
          ChatGPT Plugin to get the {paramsObject?.method} result of {paramsObject?.number1} and {searchParams?.number2}
        </p> 
        <div className="meta">
          <div className="info">
            <span>Result</span>
            <span className="region">
              <strong> {paramsObject?.result} </strong>
            </span>
          </div>
          <div className="info">
            <span>Current Result</span>
            <strong>
              {paramsObject?.result } 
            </strong>
          </div>
        </div>
      </main>
      <div>
          Math Plugin - ChatGPT Plugin to displaying {paramsObject?.method} result of number {searchParams?.number1} and {searchParams?.number2} is {searchParams?.result}
      </div>
  </div>
  )

}