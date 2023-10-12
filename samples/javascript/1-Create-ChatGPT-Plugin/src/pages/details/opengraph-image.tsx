import { ImageResponse } from "next/server"; 

export const runtime = "edge";

const stats = [
  {
    name: "TEMPERATURE",
    attr: "temp_c",
    unit: "°C",
  },
  {
    name: "WIND",
    attr: "wind_mph",
    unit: "mph",
  },
  {
    name: "HUMIDITY",
    attr: "humidity",
    unit: "%",
  },
];

export default async function DetailsOG({
  params,
}: {
  params: { number1: string, number2: string, result: string, method: string };
}) { 


  return new ImageResponse(
    (
      <div
        style={{
          height: "100%",
          width: "100%",
          display: "flex",
          textAlign: "center",
          alignItems: "center",
          justifyContent: "center",
          flexDirection: "column",
          flexWrap: "nowrap",
          fontFamily: "Clash",
          backgroundColor: "white",
          backgroundImage: "radial-gradient(circle at 25px 25px, lightgray 2%, transparent 0%), radial-gradient(circle at 75px 75px, lightgray 2%, transparent 0%)",
          backgroundSize: "100px 100px",
        }}
      >
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            marginBottom: "-30px",
            fontSize: 100,
          }}
        >
          
        </div>
        <div tw="flex flex-col items-center justify-center mt-10">
          <b
            style={{
              fontSize: 60,
              color: "black",
              lineHeight: 1.8,
            }}
          >
            ChatGPT Plugin to get the ${params.method} result of ${params.number1} and ${params.number2}
          </b>
          <div
            tw="flex"
            style={{
              fontSize: 48,
              color: "black",
            }}
          >
            <div className="info">
              <span>Result</span>
              <span className="region">
                <strong> ${params.result} </strong>
              </span>
          </div>
          </div>
        </div>
      </div>
    ),
    {
      width: 1200,
      height: 600, 
      emoji: "blobmoji",
    }
  );
}
