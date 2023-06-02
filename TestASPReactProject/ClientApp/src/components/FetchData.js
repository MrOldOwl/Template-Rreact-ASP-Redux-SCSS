import React, { useEffect, useRef, useState } from 'react';

const apiSocket = `wss://${window.location.hostname}:${window.location.port}/socket`;

function FetchData() {
    const [forecasts, setForecasts] = useState([])
    const [loading, setLoading] = useState(true)

    const [number, setNumber] = useState(5)
    const socket = useRef(null)

    useEffect(() => {
        async function fetchData() {
            const api = `weatherforecast?count=${number}`;
            await fetch(api)
                .then(res => res.json())
                .then(js => {
                    setForecasts(js);
                    setLoading(false);
                    console.log(number)
                })
                .catch(ex => console.log(ex))
        }
        fetchData()
    }, [number]);


    useEffect(() => {
        const message = (event) => {
            console.log("Data:", event.data);
        };

        const close = (event) => {
            if (event.wasClean) {
                socket.current = null;
            } else {
                socket.current = create();
            }
        }

        const create = () => {
            const soc = new WebSocket(apiSocket);
            soc.onmessage = message;
            soc.onclose = close;

            return soc;
        }

        socket.current = create();
    }, []);


    const numberChange = (e) => {
        setLoading(true)
        setNumber(Number(e.target.value))
    }

    const renderForecastsTable = (forecasts) => {
        return (
            <table className="table table-striped" aria-labelledby="tableLabel">
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>Temp. (C)</th>
                        <th>Temp. (F)</th>
                        <th>Summary</th>
                    </tr>
                </thead>
                <tbody>
                    {forecasts.map(forecast =>
                        <tr key={forecast.date}>
                            <td>{forecast.date}</td>
                            <td>{forecast.temperatureC}</td>
                            <td>{forecast.temperatureF}</td>
                            <td>{forecast.summary}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    return (
        <div>
            <input type="text" value={number} onChange={numberChange} />
            <h1 id="tableLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {loading
                ? <p><em>Loading...</em></p>
                : renderForecastsTable(forecasts)
            }
        </div>
    );
}

export default FetchData;