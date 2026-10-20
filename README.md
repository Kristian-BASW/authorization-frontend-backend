# Exercise: Login with React, AuthProvider, and JWT

Build a login flow in React. The user enters a username and password in a form, and an `AuthProvider` sends the credentials to the backend. When login succeeds, the backend returns a JWT, which you must store in `sessionStorage`.

## 1. Start the project

Run the following command from the project root:

```sh
docker compose up -d --build
```

- Frontend: http://localhost:3500
- Backend and Swagger: http://localhost:3501

Create a user through `POST /register` in Swagger to test login. Check the endpoint's schema for the required fields.

After changing the source code, run the command again to rebuild the containers.

## 2. Create a LoginRequest model

Create a TypeScript type or interface named `LoginRequest` in the client.

- The model must contain `username` and `password`.
- Both fields must have the type `string`.
- Use the model to type the credentials sent to the backend.

Also create a `LoginResponse` model describing the backend's response, with a `token` field of type `string`.

## 3. Build the login form in App.tsx

Create a form with:

- An input field for `username`.
- An input field for `password` that hides the entered characters.
- A login button.

Give both fields an associated label, and use React state to track their values. Both fields must be required.

When the form is submitted, prevent the browser's default page reload and collect the values in a `LoginRequest` object. The form must call the login function provided by your AuthProvider.

## 4. Create an AuthProvider

Use React Context to create an `AuthProvider` that tracks the user's login status. Wrap `App` in the provider so that components can access it.

The provider must expose the following through context:

- `token`: The current JWT, or `null` if the user is logged out.
- `login(request: LoginRequest)`: An asynchronous function that attempts to log the user in.
- `logout()`: A function that logs the user out.

Create a `useAuth` hook that components can use to access the values and functions from context.

The HTTP request and token handling must live in AuthProvider. `App.tsx` must use context to perform login and display the current login status.

## 5. Send credentials to the backend

Implement the login function in AuthProvider using `fetch`.

| Setting | Value |
| --- | --- |
| URL | `http://localhost:3501/login` |
| HTTP method | `POST` |
| Content-Type | `application/json` |

The request body must be JSON with this structure:

```json
{
  "username": "your-username",
  "password": "your-password"
}
```

When login succeeds, the backend responds with HTTP `200` and a JSON object containing the `token` field. Incorrect credentials result in HTTP `401`.

- Check the HTTP status before treating the response as a successful login.
- Display a clear error message for an incorrect username or password.
- Handle cases where the server cannot be reached.
- Show that login is in progress and disable the login button while the request is being processed.

## 6. Store the token in sessionStorage

When login succeeds, AuthProvider must:

1. Read the token from the backend's response.
2. Store the token in `sessionStorage` using the key `token`.
3. Update its React state so that the interface shows the user as logged in.

When AuthProvider initializes, it must read any existing token from `sessionStorage` and use it as its initial state. This should preserve the user's login status when the page is reloaded in the same tab.

Store only the token. Do not store the username and password request in `sessionStorage`. Clear the password field after a successful login.

## 7. Implement logout

Display a logout button when the user is logged in. When clicked, AuthProvider must remove the token from both `sessionStorage` and React state, and the interface must show the login form again.

## 8. Test your solution

- Correct credentials return a token and show the user as logged in.
- The token is visible under Session Storage in the browser's developer tools.
- Incorrect credentials display an error and do not store a token.
- An unreachable backend displays an error, and the login button becomes available again.
- Reloading the page in the same tab preserves login status.
- Logging out removes the token and displays the login form again.

## Extra exercise: Use the token

Call the backend's protected endpoint, `GET /api/Users`. Get the token through AuthProvider and send it in the request's `Authorization` header using the format `Bearer <token>`.

Compare the responses with and without a valid token. Handle HTTP `401` when the token has expired or is invalid by clearing the login state and asking the user to log in again.

