import { NextRequest, NextResponse } from "next/server"; 

/**
 * @swagger 
 * /api/sqrt:
 *   get:
 *    description: Take the square root of a number
 *    tags: [
 *      "Square Root"
 *    ]
 *    operationId: sqrt
 *    produces: [
 *      "application/json",
 *      "text/plain"
 *    ]
 *    parameters:
 *          - in: query
 *            name: number1
 *            description: The number to calculate the square root of
 *            required: true
 *            type: integer
 *            format: int32 
 *    responses:
 *       200:
 *          description: Returns the square root of the number
 *          scehma:
 *             type: string
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

    console.log("Number 1 -- ", number1); 


    // if req is null Or query to accept only int or float values , return error
    if (!number1 || isNaN(parseInt(number1 as string))) {
       return NextResponse.json({ error: 'Please pass numbers on the query string' }, { status: 500 }) 
    } 

    // if req is not null, return sqrt of numnber1
    const sqrt = Math.sqrt(parseInt(number1 as string));
    console.log("sqrt -- ", sqrt);
    return NextResponse.json(sqrt, { 
        status: 200, 
        headers: {
            "content-type": "text/plain", 
          },
    });  
}
