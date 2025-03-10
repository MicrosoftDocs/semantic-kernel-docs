
import React from "react";
import { DetailsPage } from '../../views/DetailsPage'; 
import AdaptiveCardRenderer from '../../components/AdaptiveCardRenderer';
import LanguageInfoCard from '../../components/LanguageInfoCard';
import { PageHeading } from './styles'; 

export default function Details() {   
 
  return (<div>
    <title> This is details page </title>
    <PageHeading> This is details page </PageHeading>
    <DetailsPage /> 
    
    <AdaptiveCardRenderer/>

    <LanguageInfoCard/>

  </div>);
}
