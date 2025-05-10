import { useEffect, useState } from 'react';
import NavMenu from './NavMenu.tsx'
import OfferWindow from './OfferWindow.tsx'
import axios from 'axios';

export default function MainPage() {

    const [tradesmen, setTradesmen] = useState<any>([]);

    useEffect(() => {
        (async () => await Load())();
      }, []);

      async function Load() {

        await axios.post("https://localhost:8081/api/TradesMan", {
    
            filter: {}

        }, {
            timeout: 5000,
            headers: {
                'Content-Type': 'application/json',
                accept: 'application/json', // If you receieve JSON response.
            }
        }).then(response => {
          console.log("Got them!");
          setTradesmen(response.data);
      });
    }
      
    return (
    <section>
            <NavMenu/>
            <h1>Current offers</h1>
            <br></br>
            <table className="table table-dark" align="center">
                <thead>
                    <tr>
                        <th scope="col">
                                Id
                        </th>
                        <th scope="col">
                                Name
                        </th>
                        <th scope="col">
                                Description
                        </th>
                    </tr>
                </thead>
                {tradesmen?.map(function fn(tradesman: any) {
                    return (
                        <tbody>
                            <tr>
                                <th scope="row">{tradesman.id} </th>
                                <td>{tradesman.name}</td>
                                <td>{tradesman.description}</td>
                            </tr>
                        </tbody>
                    );
                })}
            </table>
    </section>
    )
}