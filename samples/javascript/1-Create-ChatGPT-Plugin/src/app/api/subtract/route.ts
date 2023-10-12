import { NextRequest, NextResponse } from "next/server";

/**
 * @swagger 
 * /api/subtract:
 *   get:
 *    description: To add two numbers
 *    tags: [
 *      "Add"
 *    ]
 *    operationId: suntract
 *    produces: [
 *      "application/json",
 *      "text/plain"
 *    ]
 *    parameters:
 *          - in: query
 *            name: number1
 *            description: The first number to subtract from
 *            required: true
 *            type: integer
 *            format: int32
 *          - in: query
 *            name: number2
 *            description: The second number to subtract away
 *            required: true
 *            type: integer
 *            format: int32 
 *    responses:
 *       200:
 *          description: Returns the difference of the two numbers
 *       500:
 *          description: Error
 *       400:
 *          description: Bad Request
 *       404:
 *          description: Not Found
 * 
 */
export async function GET(req: NextRequest) {     
    let number1 = req.nextUrl.searchParams.get("number1");
    let number2 = req.nextUrl.searchParams.get("number2");  

    console.log("Number 1 -- ", number1);
    console.log("Number 2 -- ", number2);


    // if req is null Or query to accept only int or float values , return error
    if ((!number1 || !number1) || isNaN(parseInt(number1 as string)) || isNaN(parseInt(number2 as string)) ) {
       return NextResponse.json({ error: 'Please pass two numbers on the query string' }, { status: 500 }) 
    } 

    // if req is not null, return the difference of the two numbers
    const difference = parseInt(number1 as string) - parseInt(number2 as string);
    console.log("Sum -- ", difference);
    return NextResponse.json(difference, { 
        status: 200, 
        headers: {
            "content-type": "text/plain", 
          },
    });  
}
