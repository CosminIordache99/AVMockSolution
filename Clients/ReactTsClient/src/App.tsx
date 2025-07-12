import { useState, useEffect } from 'react'

interface WeatherForecast {
    date: string
    temperatureC: number
    summary: string
}

function App() {
    const [forecasts, setForecasts] = useState<WeatherForecast[]>([])
    const [error, setError] = useState<string | null>(null)

    useEffect(() => {
        const base = 'http://localhost:5046';

        fetch(`${base}/WeatherForecast`)
            .then(res => {
                if (!res.ok) throw new Error(`HTTP ${res.status}`)
                return res.json() as Promise<WeatherForecast[]>
            })
            .then(data => {
                setForecasts(data)
            })
            .catch(err => {
                console.error(err)
                setError(err.message)
            })
    }, [])

    return (
        <div className="p-4">
            <h1 className="text-2xl mb-4">Weather Forecast</h1>
            {error && <div className="text-red-500">Error: {error}</div>}
            {!error && forecasts.length === 0 && <div>Loading…</div>}
            <ul>
                {forecasts.map(f => (
                    <li key={f.date} className="mb-2">
                        <strong>{new Date(f.date).toLocaleDateString()}</strong>:&nbsp;
                        {f.temperatureC}°C — {f.summary}
                    </li>
                ))}
            </ul>
        </div>
    )
}

export default App