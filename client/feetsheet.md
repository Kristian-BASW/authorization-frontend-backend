# Client login cheatsheet

This is a reference implementation for the [login exercise](../README.md). The backend is already implemented. Make the changes below in the client; no server changes are needed.

All file paths below are relative to `client/`. Apply all five sections before running the app.

## Files to create or update

| File | Action | Purpose |
| --- | --- | --- |
| `src/models/login-request.ts` | Check the existing file | Type the credentials sent to the API |
| `src/models/login-response.ts` | Create | Type the API's login response |
| `src/auth/AuthProvider.tsx` | Create | Handle login, logout, token state, and storage |
| `src/App.tsx` | Replace | Display the login form and login status |
| `src/frontend.tsx` | Replace | Provide authentication context to the app |

The flow is: form → `useAuth().login(request)` → `POST /login` → token in `sessionStorage` and React state → updated interface.

## 1. Login request model

**File: `src/models/login-request.ts`**

This file already exists. Check that it contains the following fields; you do not need a second request model.

```tsx
export interface LoginRequest {
  username: string;
  password: string;
}
```

## 2. Login response model

**File: `src/models/login-response.ts`**

Create this file to describe the JSON returned by a successful login.

```tsx
export interface LoginResponse {
  token: string;
}
```

## 3. AuthProvider and useAuth

**File: `src/auth/AuthProvider.tsx`**

Create the `auth` folder and this file. The provider handles the HTTP request, stores the token, and exposes login state through React Context. `useAuth` gives components access to that context. The initial state reads from `sessionStorage` so that reloading the same tab preserves login status.

```tsx
import {
  createContext,
  useContext,
  useState,
  type ReactNode,
} from "react";
import type { LoginRequest } from "../models/login-request";
import type { LoginResponse } from "../models/login-response";

interface AuthContextValue {
  token: string | null;
  login: (request: LoginRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() =>
    sessionStorage.getItem("token"),
  );

  async function login(request: LoginRequest) {
    let response: Response;

    try {
      response = await fetch("http://localhost:3501/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(request),
      });
    } catch {
      throw new Error("Could not reach the server. Please try again.");
    }

    if (response.status === 401) {
      throw new Error("Incorrect username or password.");
    }

    if (!response.ok) {
      throw new Error("Login failed. Please try again later.");
    }

    const data: LoginResponse = await response.json();

    if (typeof data.token !== "string" || !data.token) {
      throw new Error("The server returned an invalid token.");
    }

    sessionStorage.setItem("token", data.token);
    setToken(data.token);
  }

  function logout() {
    sessionStorage.removeItem("token");
    setToken(null);
  }

  return (
    <AuthContext.Provider value={{ token, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used inside an AuthProvider.");
  }

  return context;
}
```

## 4. Login form

**File: `src/App.tsx`**

Replace the contents of this file. The form owns the input values, loading state, and error message. It passes a `LoginRequest` to the provider and clears the password after a successful login. The existing `index.css` already contains the classes used below.

```tsx
import "./index.css";
import { useState, type FormEvent } from "react";
import minion from "./assets/minion.png";
import { useAuth } from "./auth/AuthProvider";
import type { LoginRequest } from "./models/login-request";

export function App() {
  const { token, login, logout } = useAuth();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (isLoading) return;

    setError("");
    setIsLoading(true);

    const request: LoginRequest = { username, password };

    try {
      await login(request);
      setPassword("");
    } catch (error) {
      setError(error instanceof Error ? error.message : "Login failed.");
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <div className="app">
      <div className="logo-container">
        <img src={minion} alt="Minion" className="logo bun-logo" />
      </div>

      <h1>Login</h1>

      {token ? (
        <div>
          <p role="status">You are logged in.</p>
          <button className="send-button" onClick={logout}>
            Log out
          </button>
        </div>
      ) : (
        <form className="login-form" onSubmit={handleSubmit} aria-busy={isLoading}>
          <label htmlFor="username">Username</label>
          <input
            id="username"
            name="username"
            autoComplete="username"
            value={username}
            onChange={event => setUsername(event.target.value)}
            disabled={isLoading}
            required
          />

          <label htmlFor="password">Password</label>
          <input
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            value={password}
            onChange={event => setPassword(event.target.value)}
            disabled={isLoading}
            required
          />

          {error && <p className="login-error" role="alert">{error}</p>}

          <button className="send-button" type="submit" disabled={isLoading}>
            {isLoading ? "Logging in…" : "Log in"}
          </button>
        </form>
      )}
    </div>
  );
}

export default App;
```

## 5. Wrap App in AuthProvider

**File: `src/frontend.tsx`**

Replace the contents of this file. This project mounts React in `frontend.tsx`, so wrap `App` here. Every component calling `useAuth` must be rendered inside `AuthProvider`. Keep the conditional `import.meta.hot` handling so the entry point works in both development and production.

```tsx
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { App } from "./App";
import { AuthProvider } from "./auth/AuthProvider";

const elem = document.getElementById("root")!;
const app = (
  <StrictMode>
    <AuthProvider>
      <App />
    </AuthProvider>
  </StrictMode>
);

const root = import.meta.hot
  ? (import.meta.hot.data.root ??= createRoot(elem))
  : createRoot(elem);

root.render(app);
```

## 6. Run and verify

From the project root, rebuild and start the containers:

```sh
docker compose up -d --build
```

Open [Swagger](http://localhost:3501/swagger) and create a user with `POST /register`. Use that account in the [frontend](http://localhost:3500).

The request runs in the browser, so it uses `http://localhost:3501/login`. The Docker service name `api` is used for communication between containers and is not the API address for browser requests.

Check the following:

1. Submit incorrect credentials. An error should appear, and the login button should become available again.
2. Submit correct credentials. The app should display “You are logged in.”
3. In developer tools, open **Application → Session Storage → the frontend's origin**. There should be a `token` entry. The credentials must not be stored there.
4. Reload the page in the same tab. The app should still show the user as logged in.
5. Click **Log out**. The `token` entry should disappear, and the form should return with an empty password field.
6. Stop the API with `docker compose stop api` and attempt to log in. A connection error should appear, and the button should become available again. Restart it with `docker compose up -d api`.

To check the client separately, run these commands from `client/`:

```sh
bun run build
bunx tsc --noEmit
```

Changes to the client require another Docker build when using Compose. Alternatively, keep the API running and use `bun run dev` from `client/` for local development at `http://localhost:3000`.

## How the pieces fit together

- `LoginRequest` describes the outgoing username and password; `LoginResponse` describes the incoming token.
- `AuthProvider` sends the request, checks the response, and updates both storage and state.
- `App` calls `login` through context and displays errors thrown by the provider.
- React state updates the interface immediately. Writing to `sessionStorage` alone does not trigger a React render.
- `logout` clears the stored token and the state. It does not revoke the JWT on the server.
- A stored token preserves the interface's login status. The backend must still validate the token on protected requests, including its expiry. Handling expired tokens is part of the extra exercise in the main README.

## Verification of this example

The code above passed TypeScript checking and a production build. It was also tested in Chrome against the existing backend: incorrect credentials, network failure, successful login, token storage, reload, and logout. The stored token successfully authorized a request to `GET /api/Users`.

Testing used a temporary copy of the client. The implementation is included here as reference code; it has not been added to the client's source files.

