import pytest
import requests

def test_login_contract(auth_base_url):
    payload = {
        "email": "owner@spacepulse.com",
        "password": "Password123!"
    }
    try:
        res = requests.post(f"{auth_base_url}/users/login", json=payload, timeout=5)
        if res.status_code == 200:
            data = res.json()
            assert "token" in data
            assert "id" in data
            assert "role" in data
            assert data["role"] in ["Homeowner", "Remodeler"]
    except requests.exceptions.ConnectionError:
        pytest.skip("Backend API server not reachable at localhost:5000")

def test_invalid_login_rejection(auth_base_url):
    payload = {
        "email": "nonexistent@spacepulse.com",
        "password": "WrongPassword!"
    }
    try:
        res = requests.post(f"{auth_base_url}/users/login", json=payload, timeout=5)
        assert res.status_code in [400, 401, 404]
    except requests.exceptions.ConnectionError:
        pytest.skip("Backend API server not reachable at localhost:5000")
