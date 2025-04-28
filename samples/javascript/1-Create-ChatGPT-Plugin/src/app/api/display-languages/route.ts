import { NextRequest, NextResponse } from "next/server"; 
import languages  from "../../../data/supportedlanguages";
import json2md from "json2md"; 

/**
 * @swagger 
 * /api/display-languages:
 *   get:
 *    description: Returns the Markdown for list of supported languages
 *    tags: [
 *      "Add"
 *    ]
 *    operationId: add
 *    produces: [
 *      "application/json",
 *      "text/plain",
 *      "text/markdown"
 *    ] 
 *    responses:
 *       200:
 *          description: Returns the Markdown for list of supported languages
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
    console.log("-- Request to display languages --");


    // if req is null Or query to accept only int or float values , return error
    if (languages === null || languages === undefined) {
       return NextResponse.json({ error: 'No languages to display' }, { status: 500 }) 
    }  

    let josnToMarkDown = '|  Language  |  TranslatedVersion  |\n ';
    josnToMarkDown += '|----------|-------------------|\n';

    // Convert list to json object
    for (var key in languages ){  
        // @ts-ignore
        josnMarkDown += '  | ' + key + "  |  " + languages[key] + ' | \n ';  
    }  
    console.log("-- josnMarkDown --", josnToMarkDown);

    return NextResponse.json(josnToMarkDown, { 
        status: 200, 
        headers: {
            // "content-type": "text/plain", 
            //"content-type": "application/json"
            "content-type": "text/markdown"
          },
    });  
}
