import pytest
import requests

BASE_URL = "http://localhost:5000/api/v1"
AUTH_BASE_URL = "http://localhost:5000/api"

@pytest.fixture(scope="session")
def base_url():
    return BASE_URL

@pytest.fixture(scope="session")
def auth_base_url():
    return AUTH_BASE_URL

@pytest.fixture(scope="session")
def homeowner_auth_token(auth_base_url):
    login_payload = {
        "email": "owner@spacepulse.com",
        "password": "Password123!"
    }
    try:
        res = requests.post(f"{auth_base_url}/users/login", json=login_payload, timeout=5)
        if res.status_code == 200:
            return res.json().get("token")
    except Exception:
        pass
    return "mock_homeowner_jwt_token"

@pytest.fixture(scope="session")
def remodeler_auth_token(auth_base_url):
    login_payload = {
        "email": "builder@spacepulse.com",
        "password": "Password123!"
    }
    try:
        res = requests.post(f"{auth_base_url}/users/login", json=login_payload, timeout=5)
        if res.status_code == 200:
            return res.json().get("token")
    except Exception:
        pass
    return "mock_remodeler_jwt_token"
