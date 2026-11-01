from flask import Flask, jsonify, request
from flask_cors import CORS
from dotenv import load_dotenv
import os
import requests

# Load the private API key from the .env file beside this app.py file.
load_dotenv()

app = Flask(__name__)
CORS(app)

OPENWEATHER_URL = "https://api.openweathermap.org/data/2.5/weather"


@app.get("/")
def home():
    return jsonify({
        "message": "Weather backend is running.",
        "usage": "/weather?lat=-1.94&lon=30.06"
    })


@app.get("/weather")
def get_weather():
    # Read the coordinates sent by the Unity application.
    try:
        latitude = float(request.args.get("lat", ""))
        longitude = float(request.args.get("lon", ""))
    except ValueError:
        return jsonify({"error": "Please provide valid lat and lon numbers."}), 400

    if not (-90 <= latitude <= 90 and -180 <= longitude <= 180):
        return jsonify({"error": "Coordinates are outside the valid range."}), 400

    # The API key stays on this server, not inside the Unity WebGL build.
    api_key = os.getenv("OPENWEATHER_API_KEY")
    if not api_key:
        return jsonify({
            "error": "The server is missing OPENWEATHER_API_KEY in its .env file."
        }), 500

    try:
        response = requests.get(
            OPENWEATHER_URL,
            params={
                "lat": latitude,
                "lon": longitude,
                "appid": api_key,
                "units": "metric"
            },
            timeout=15
        )

        # Pass OpenWeather's error status and message back to Unity.
        if not response.ok:
            try:
                details = response.json()
            except ValueError:
                details = {"message": response.text}
            return jsonify({
                "error": "OpenWeather request failed.",
                "details": details
            }), response.status_code

        return jsonify(response.json())

    except requests.RequestException as error:
        return jsonify({
            "error": "Could not contact OpenWeather.",
            "details": str(error)
        }), 502


if __name__ == "__main__":
    # Local testing only. For deployment, use a production WSGI server.
    app.run(host="127.0.0.1", port=5000, debug=True)

