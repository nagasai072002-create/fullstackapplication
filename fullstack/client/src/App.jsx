import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [session, setSession] = useState(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [loginError, setLoginError] = useState('');
    const [candidates, setCandidates] = useState([]);
    const [isLoadingCandidates, setIsLoadingCandidates] = useState(false);
    const [candidateError, setCandidateError] = useState('');

    useEffect(() => {
        if (session?.user?.role !== 'Admin') {
            return undefined;
        }

        const controller = new AbortController();
        async function loadCandidates() {
            try {
                const response = await fetch('/api/candidates', {
                    headers: { Authorization: `Bearer ${session.accessToken}` },
                    signal: controller.signal,
                });

                if (!response.ok) {
                    if (response.status === 401) {
                        setSession(null);
                        setCandidates([]);
                        setLoginError('Your session expired. Please sign in again.');
                    }
                    throw new Error(`Could not load candidates (HTTP ${response.status}).`);
                }

                setCandidates(await response.json());
            } catch (error) {
                if (error.name !== 'AbortError') {
                    setCandidateError(error.message || 'Could not load candidates.');
                }
            } finally {
                if (!controller.signal.aborted) {
                    setIsLoadingCandidates(false);
                }
            }
        }

        loadCandidates();
        return () => controller.abort();
    }, [session]);

    async function handleLogin(event) {
        event.preventDefault();
        setIsSubmitting(true);
        setLoginError('');

        try {
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password }),
            });

            if (!response.ok) {
                setLoginError(response.status === 401
                    ? 'Invalid username or password.'
                    : `Login failed (HTTP ${response.status}).`);
                return;
            }

            const result = await response.json();
            if (!['Admin', 'Candidate'].includes(result.user?.role)) {
                setLoginError('This account has an unsupported role.');
                return;
            }

            setCandidates([]);
            setCandidateError('');
            setIsLoadingCandidates(result.user.role === 'Admin');
            setSession(result);
            setPassword('');
        } catch {
            setLoginError('Could not reach the server. Check that the API is running and try again.');
        } finally {
            setIsSubmitting(false);
        }
    }

    function handleLogout() {
        setSession(null);
        setCandidates([]);
        setCandidateError('');
        setLoginError('');
        setUsername('');
        setPassword('');
    }

    if (!session) {
        return (
            <main className="page-shell">
                <section className="login-card" aria-labelledby="login-title">
                    <p className="eyebrow">Fullstack application</p>
                    <h1 id="login-title">Sign in</h1>
                    <p className="intro">Use your account to continue to your portal.</p>
                    <form onSubmit={handleLogin}>
                        <label htmlFor="username">Username</label>
                        <input
                            autoComplete="username"
                            id="username"
                            name="username"
                            onChange={event => setUsername(event.target.value)}
                            required
                            value={username}
                        />
                        <label htmlFor="password">Password</label>
                        <input
                            autoComplete="current-password"
                            id="password"
                            name="password"
                            onChange={event => setPassword(event.target.value)}
                            required
                            type="password"
                            value={password}
                        />
                        {loginError && <p className="error-message" role="alert">{loginError}</p>}
                        <button className="primary-button" disabled={isSubmitting} type="submit">
                            {isSubmitting ? 'Signing in…' : 'Sign in'}
                        </button>
                    </form>
                </section>
            </main>
        );
    }

    if (session.user.role === 'Candidate') {
        return (
            <main className="page-shell">
                <section className="welcome-card" aria-labelledby="welcome-title">
                    <p className="eyebrow">Candidate portal</p>
                    <h1 id="welcome-title">Welcome to Candidate Portal</h1>
                    <p className="intro">Signed in as {session.user.displayName}.</p>
                    <button className="secondary-button" onClick={handleLogout} type="button">Log out</button>
                </section>
            </main>
        );
    }

    return (
        <main className="page-shell admin-page">
            <header className="page-header">
                <div>
                    <p className="eyebrow">Admin portal</p>
                    <h1>Welcome to Admin Portal</h1>
                    <p className="intro">Signed in as {session.user.displayName}.</p>
                </div>
                <button className="secondary-button" onClick={handleLogout} type="button">Log out</button>
            </header>
            <section className="list-card" aria-labelledby="candidate-list-title">
                <h2 id="candidate-list-title">Candidates</h2>
                {isLoadingCandidates && <p className="status-message">Loading candidates…</p>}
                {candidateError && <p className="error-message" role="alert">{candidateError}</p>}
                {!isLoadingCandidates && !candidateError && candidates.length === 0 && (
                    <p className="status-message">No candidates found.</p>
                )}
                {candidates.length > 0 && (
                    <div className="table-scroll">
                        <table>
                            <thead>
                                <tr><th>Name</th><th>Username</th><th>Role</th></tr>
                            </thead>
                            <tbody>
                                {candidates.map(candidate => (
                                    <tr key={candidate.id}>
                                        <td>{candidate.displayName}</td>
                                        <td>{candidate.username}</td>
                                        <td><span className="role-badge">{candidate.role}</span></td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </section>
        </main>
    );
}

export default App;