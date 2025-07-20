import { useState, useEffect } from 'react'

interface ThreatInfoDto {
    filePath: string
    threatName: string
}

interface ScanSessionDto {
    startTimestamp: string
    stopTimestamp: string
    reason: string
    threats: ThreatInfoDto[]
}

interface RealTimeThreatDto {
    timestamp: string
    filePath: string
    threatName: string
}

interface RealTimeStatusDto {
    timestamp: string
    isEnabled: boolean
}

interface ScanResultDto {
    onDemandSessions: ScanSessionDto[]
    realTimeThreats: RealTimeThreatDto[]
    realTimeStatuses: RealTimeStatusDto[]
}

function App() {
    const [sessions, setSessions] = useState<ScanSessionDto[]>([])
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)

    const baseUrl = 'http://localhost:5046/api'

    const fetchSessions = async () => {
        setLoading(true)
        setError(null)
        try {
            const res = await fetch(`${baseUrl}/events`)
            if (!res.ok) throw new Error(`HTTP ${res.status}`)
            const result = (await res.json()) as ScanResultDto
            setSessions(result.onDemandSessions)
        } catch (err: any) {
            console.error(err)
            setError(err.message)
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        fetchSessions()
    }, [])

    const callEndpoint = async (path: string) => {
        setError(null)
        try {
            const res = await fetch(`${baseUrl}${path}`, { method: 'POST' })
            if (!res.ok) throw new Error(`HTTP ${res.status}`)
            await fetchSessions()
        } catch (err: any) {
            console.error(err)
            setError(err.message)
        }
    }

    const clearSessions = async () => {
        setError(null)
        try {
            const res = await fetch(`${baseUrl}/events`, { method: 'DELETE' })
            if (!res.ok) throw new Error(`HTTP ${res.status}`)
            setSessions([])
        } catch (err: any) {
            console.error(err)
            setError(err.message)
        }
    }

    return (
        <div className="p-4 max-w-3xl mx-auto">
            <h1 className="text-3xl font-bold mb-4">AVMockSolution Control Panel</h1>

            {error && <div className="mb-4 text-red-600">Error: {error}</div>}

            <div className="grid grid-cols-2 gap-2 mb-6">
                <button
                    className="bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
                    onClick={() => callEndpoint('/ondemand/start')}
                >
                    Start On‑Demand Scan
                </button>
                <button
                    className="bg-blue-600 text-white py-2 rounded hover:bg-blue-700"
                    onClick={() => callEndpoint('/ondemand/stop')}
                >
                    Stop On‑Demand Scan
                </button>
                <button
                    className="bg-green-600 text-white py-2 rounded hover:bg-green-700"
                    onClick={() => callEndpoint('/realtime/enable')}
                >
                    Enable Real‑Time
                </button>
                <button
                    className="bg-green-600 text-white py-2 rounded hover:bg-green-700"
                    onClick={() => callEndpoint('/realtime/disable')}
                >
                    Disable Real‑Time
                </button>
            </div>

            <div className="flex items-center mb-4">
                <h2 className="text-2xl font-semibold flex-1">Stored Scan Sessions</h2>
                <button
                    className="bg-gray-600 text-white py-1 px-3 rounded hover:bg-gray-700"
                    onClick={clearSessions}
                >
                    Clear All
                </button>
            </div>

            {loading && <div>Loading sessions…</div>}
            {!loading && sessions.length === 0 && <div>No sessions found.</div>}

            <ul className="space-y-4">
                {sessions.map((session, idx) => (
                    <li key={idx} className="border p-4 rounded shadow-sm">
                        <div className="mb-2">
                            <strong>Start:</strong>{' '}
                            {new Date(session.startTimestamp).toLocaleString()}
                        </div>
                        <div className="mb-2">
                            <strong>Stop:</strong>{' '}
                            {new Date(session.stopTimestamp).toLocaleString()}
                        </div>
                        <div className="mb-2">
                            <strong>Reason:</strong> {session.reason}
                        </div>
                        {session.threats.length > 0 && (
                            <div>
                                <strong>Threats:</strong>
                                <ul className="list-disc list-inside mt-1">
                                    {session.threats.map((t, ti) => (
                                        <li key={ti}>
                                            {t.filePath} — <em>{t.threatName}</em>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        )}
                    </li>
                ))}
            </ul>
        </div>
    )
}

export default App
