import React, { useEffect, useRef } from 'react'; 
import * as AdaptiveCards from "adaptivecards";  
import { CardContainer } from './styles'; 

function LanguageInfoCard(props: any) {
    const containerRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const adaptiveCard = new AdaptiveCards.AdaptiveCard();
        var card = {
            "type": "AdaptiveCard",
            "version": "1.0",
            "body": [
                {
                    "type": "ColumnSet",
                    columns: [
                        {
                            "type": "Column",
                            "width": "auto",
                            "items": [
                                {
                                    "type": "Image",
                                    "url": '/icons/logo.svg',
                                    "altText": "Microsoft Logo",
                                    "size": "small",
                                    "width": "30px",
                                    "height": "30px"
                                }
                            ], 
                        },
                        {
                            "type": "Column",
                            "width": "auto",
                            "verticalContentAlignment": "Center",
                            "items": [
                                {
                                    "type": "TextBlock",
                                    "weight": "bolder",
                                    "text": "Language info"
                                }
                            ]
                        }
                    ]
                }, 
                {
                    "type": "TextBlock",
                    "wrap": true,
                    "text": "Would you like to change your display language for your account?"
                }
            ],
            "actions": [ 
                        {
                            "type": "Action.Execute",
                            "title": "Yes",  
                        },
                        {
                            "type": "Action.Execute",
                            "title": "No Thanks", 
                        }  
                ]
        };
        
        adaptiveCard.hostConfig = new AdaptiveCards.HostConfig({
            fontFamily: "Segoe UI, Helvetica Neue, sans-serif" 
        });
    
        adaptiveCard.onExecuteAction = function(action) { alert("Hello alert"); }
    
        // Parse the card payload
        adaptiveCard.parse(card);

        const renderedCard = adaptiveCard.render();

        // Store the current ref value in a variable
        const currentContainer = containerRef.current;

        if (currentContainer && renderedCard) {
            currentContainer.appendChild(renderedCard);
        }

        // Cleanup function: use the variable in the cleanup
        return () => {
            if (currentContainer && renderedCard) {
                currentContainer.removeChild(renderedCard);
            }
        };
    }, []); // Empty dependency array means this effect will run once when the component mounts


    return <CardContainer ref={containerRef}></CardContainer>; 

}

export default LanguageInfoCard;