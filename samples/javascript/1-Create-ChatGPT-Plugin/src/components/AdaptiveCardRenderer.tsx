import React, { useEffect, useRef } from 'react'; 
import * as AdaptiveCards from "adaptivecards";  
import { CardContainer } from './styles';

function AdaptiveCardRenderer(props: any) {
    const containerRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        const adaptiveCard = new AdaptiveCards.AdaptiveCard();
        var card = {
            "type": "AdaptiveCard",
            "version": "1.0",
            "body": [
                {
                    "type": "Image",
                    "url": "http://adaptivecards.io/content/adaptive-card-50.png"
                },
                {
                    "type": "TextBlock",
                    "text": "Hello its first Adaptive Cards!"
                }
            ],
            "actions": [
                {
                    "type": "Action.OpenUrl",
                    "title": "Learn more",
                    "url": "http://adaptivecards.io"
                },
                {
                    "type": "Action.OpenUrl",
                    "title": "GitHub",
                    "url": "http://github.com/"
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

export default AdaptiveCardRenderer;