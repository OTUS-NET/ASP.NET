import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import BasicApp from './BasicApp.tsx'
import RouteParamsApp from './RouteParamsApp.tsx'
import UseNavigateApp from './UseNavigateApp.tsx'
import OutletApp from './OutletApp.tsx'
import NavigateApp from './NavigateApp.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    {/* <BasicApp /> */}
    {/* <RouteParamsApp /> */}
    {/* <UseNavigateApp /> */}
    {/* <OutletApp /> */}
    <NavigateApp/>
  </StrictMode>,
)
