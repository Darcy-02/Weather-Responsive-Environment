# Weather-Responsive-Environment





\## Technologies Used



\- \*\*Python\*\* – backend programming language

\- \*\*Flask\*\* – creates the backend API

\- \*\*Flask-CORS\*\* – allows requests from the Unity WebGL application

\- \*\*OpenWeather API\*\* – provides current weather information

\- \*\*Requests\*\* – sends requests to OpenWeather

\- \*\*Gunicorn\*\* – runs the Flask application in production

\- \*\*Render\*\* – hosts the backend online

\- \*\*Unity and C#\*\* – send requests and display weather information





\## Backend Setup



The Flask backend is inside the `Backend` folder.



\### 1. Install Dependencies



The `requirements.txt` file contains the Python packages required by the backend:



```text

Flask

flask-cors

python-dotenv

requests

gunicorn

```



Install them using:



```bash

pip install -r requirements.txt

```



\### 2. API Key Security



The backend uses an OpenWeather API key to retrieve weather information.



The API key is stored in an environment variable called:



```text

OPENWEATHER\_API\_KEY

```



For local development, the key is stored in a `.env` file.



The `.env` file is excluded from Git using `.gitignore` to prevent private credentials from being uploaded to GitHub.



The API key is kept on the backend instead of being included in the Unity WebGL application.



\### 3. Backend Endpoints



The Flask application provides two endpoints:



\*\*Home endpoint\*\*



```text

GET /

```



Checks whether the backend is running.



\*\*Weather endpoint\*\*



```text

GET /weather?lat=-1.94\&lon=30.06

```



Receives latitude and longitude coordinates, requests weather information from OpenWeather, and returns the result as JSON.



The backend also handles invalid coordinates, missing API keys, and failed weather requests.



\## Deployment on Render



The Flask backend was deployed as a Render Web Service.



The OpenWeather API key was added to Render through the Environment settings using the key:



```text

OPENWEATHER\_API\_KEY

```



The actual key is kept private and is not stored in this README.



\### Live Backend URL



https://weather-responsive-environmentb.onrender.com



\*\*Weather endpoint:\*\*



```text

https://weather-responsive-environmentb.onrender.com/weather

```



\## Connecting Unity to the Backend



The Unity C# `WeatherAPIService` communicates with the hosted Flask backend.



The backend URL is defined in the Unity script:



```csharp

private const string BACKEND\_URL =

&#x20;   "https://weather-responsive-environmentb.onrender.com";

```



When Unity requests weather information, it sends the coordinates to the `/weather` endpoint:



```csharp

string url =

&#x20;   $"{BACKEND\_URL}/weather?lat={latitude}\&lon={longitude}";

```



The request is handled by `UnityWebRequestSender`.



The response is then passed to `OpenWeatherResponseParser`, which converts the JSON response into `WeatherData`.



If the request succeeds, Unity receives the weather information. If it fails, an error message is returned.



\## Testing



The backend can be tested directly in a browser.



\*\*Check whether the backend is running:\*\*



https://weather-responsive-environmentb.onrender.com



\*\*Test the weather endpoint:\*\*



https://weather-responsive-environmentb.onrender.com/weather?lat=-1.94\&lon=30.06



A successful weather request should return JSON containing current weather information.



\## Conclusion



The project uses a Flask backend hosted on Render to connect the Unity weather application to the OpenWeather API. This setup keeps the API key on the server and allows the Unity WebGL application to retrieve weather information through an online endpoint.

