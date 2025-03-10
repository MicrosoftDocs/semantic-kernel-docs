export async function getResult(number1: string, number2: string)) { 
    return data;
    return fetch(
      `http://localhost:9080/api/add?number1=${number1}&number2=${number2}}`,
      { next: { revalidate: 60 } }
    ).then((res) => res.json());
  }
  