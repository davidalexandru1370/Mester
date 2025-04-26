import { useState } from 'react'
import './App.css'
import 'bootstrap/dist/css/bootstrap.min.css';
import OfferWindow from './components/OfferWindow.tsx'

function App() {

  return (
      <section>
        <h1>Current offers</h1>
        <br></br>
        <OfferWindow />
      </section>
  )
}

export default App
