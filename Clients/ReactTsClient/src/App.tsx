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
    const [realTimeThreats, setRealTimeThreats] = useState<RealTimeThreatDto[]>([])
    const [realTimeStatuses, setRealTimeStatuses] = useState<RealTimeStatusDto[]>([])
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)

    const baseUrl = 'http://localhost:5046/api'

    const fetchAll = async () => {
        setLoading(true)
        setError(null)
        try {
            const res = await fetch(`${baseUrl}/events`)
            if (!res.ok) throw new Error(`HTTP ${res.status}`)
            const result = (await res.json()) as ScanResultDto

            setSessions(result.onDemandSessions)
            setRealTimeThreats(result.realTimeThreats)
            setRealTimeStatuses(result.realTimeStatuses)
        } catch (err: any) {
            console.error(err)
            setError(err.message)
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        fetchAll()
    }, [])

    const callEndpoint = async (path: string) => {
        setError(null)
        try {
            const res = await fetch(`${baseUrl}${path}`, { method: 'POST' })
            if (!res.ok) throw new Error(`HTTP ${res.status}`)
            await fetchAll()
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

    // pick latest status
    const latestStatus = realTimeStatuses[realTimeStatuses.length - 1]

    return (
        <div className="p-4 max-w-5xl mx-auto space-y-6">
            <h1 className="text-4xl font-extrabold text-center mb-6">AV Mock Control Panel</h1>

            {error && <div className="text-red-600 font-medium">{error}</div>}

            {/* Controls */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                <button
                    className="bg-blue-600 text-white py-2 rounded shadow hover:bg-blue-700"
                    onClick={() => callEndpoint('/ondemand/start')}
                >
                    Start Scan
                </button>
                <button
                    className="bg-blue-600 text-white py-2 rounded shadow hover:bg-blue-700"
                    onClick={() => callEndpoint('/ondemand/stop')}
                >
                    Stop Scan
                </button>
                <button
                    className="bg-green-600 text-white py-2 rounded shadow hover:bg-green-700"
                    onClick={() => callEndpoint('/realtime/enable')}
                >
                    Enable RT
                </button>
                <button
                    className="bg-green-600 text-white py-2 rounded shadow hover:bg-green-700"
                    onClick={() => callEndpoint('/realtime/disable')}
                >
                    Disable RT
                </button>
            </div>

            {/* Status & Threats */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Real‑Time Status */}
                <div className="border rounded-lg p-4 shadow-sm">
                    <h2 className="text-2xl font-semibold mb-2">Real‑Time Protection</h2>
                    {latestStatus ? (
                        <div className="flex items-center space-x-3">
                            <span
                                className={`px-3 py-1 rounded-full text-white font-medium ${latestStatus.isEnabled ? 'bg-green-600' : 'bg-gray-600'
                                    }`}
                            >
                                {latestStatus.isEnabled ? 'Enabled' : 'Disabled'}
                            </span>
                            <span className="text-sm text-gray-500">
                                at {new Date(latestStatus.timestamp).toLocaleTimeString()}
                            </span>
                        </div>
                    ) : (
                        <div className="text-gray-500">No status updates yet.</div>
                    )}
                </div>

                {/* Real‑Time Threats */}
                <div className="border rounded-lg p-4 shadow-sm">
                    <h2 className="text-2xl font-semibold mb-2">Real‑Time Threats</h2>
                    {realTimeThreats.length > 0 ? (
                        <ul className="space-y-3 max-h-64 overflow-y-auto">
                            {realTimeThreats.map((t, i) => (
                                <li
                                    key={i}
                                    className="p-3 border-l-4 border-red-500 bg-red-50 rounded"
                                >
                                    <div className="text-sm text-gray-500 mb-1">
                                        {new Date(t.timestamp).toLocaleTimeString()}
                                    </div>
                                    <div className="font-medium">{t.filePath}</div>
                                    <div className="italic">{t.threatName}</div>
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <div className="text-gray-500">No real‑time threats detected.</div>
                    )}
                </div>
            </div>

            {/* On‑Demand Sessions */}
            <div>
                <div className="flex justify-between items-center mb-4">
                    <h2 className="text-2xl font-semibold">On‑Demand Scan Sessions</h2>
                    <button
                        className="bg-gray-600 text-white py-1 px-3 rounded hover:bg-gray-700"
                        onClick={clearSessions}
                    >
                        Clear All
                    </button>
                </div>

                {loading ? (
                    <div>Loading…</div>
                ) : sessions.length === 0 ? (
                    <div className="text-gray-500">No sessions found.</div>
                ) : (
                    <ul className="space-y-4">
                        {sessions.map((s, idx) => (
                            <li key={idx} className="border rounded-lg p-4 shadow-sm">
                                <div className="flex justify-between text-sm text-gray-500 mb-2">
                                    <div>
                                        <strong>Start:</strong>{' '}
                                        {new Date(s.startTimestamp).toLocaleString()}
                                    </div>
                                    <div>
                                        <strong>Stop:</strong>{' '}
                                        {new Date(s.stopTimestamp).toLocaleString()}
                                    </div>
                                </div>
                                <div className="mb-2">
                                    <strong>Reason:</strong> {s.reason}
                                </div>
                                {s.threats.length > 0 && (
                                    <div>
                                        <strong>Threats:</strong>
                                        <ul className="list-disc list-inside mt-1">
                                            {s.threats.map((t, ti) => (
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
                )}
            </div>
        </div>
    )
}

export default App
