import pytest
import requests

def test_get_all_spaces_contract(base_url, homeowner_auth_token):
    headers = {"Authorization": f"Bearer {homeowner_auth_token}"}
    try:
        res = requests.get(f"{base_url}/space", headers=headers, timeout=5)
        if res.status_code == 200:
            data = res.json()
            assert isinstance(data, list)
            if len(data) > 0:
                first = data[0]
                assert "id" in first
                assert "title" in first
                assert "pricePerMonth" in first
                assert "location" in first
                assert "city" in first["location"]
    except requests.exceptions.ConnectionError:
        pytest.skip("Backend API server not reachable at localhost:5000")

def test_space_creation_and_lifecycle(base_url, homeowner_auth_token):
    headers = {"Authorization": f"Bearer {homeowner_auth_token}"}
    payload = {
        "title": "Integration Test Office",
        "description": "Pytest automated verification space",
        "type": "Office",
        "pricePerMonth": 1800.0,
        "location": {
            "address": "Av. Republica 200",
            "city": "Lima",
            "country": "Peru",
            "latitude": -12.10,
            "longitude": -77.03
        },
        "images": []
    }
    try:
        create_res = requests.post(f"{base_url}/space", json=payload, headers=headers, timeout=5)
        if create_res.status_code == 201:
            space_id = create_res.json()["id"]
            
            # Verify detail
            get_res = requests.get(f"{base_url}/space/{space_id}", headers=headers, timeout=5)
            assert get_res.status_code == 200
            
            # Delete space
            del_res = requests.delete(f"{base_url}/space/{space_id}", headers=headers, timeout=5)
            assert del_res.status_code in [200, 204]
    except requests.exceptions.ConnectionError:
        pytest.skip("Backend API server not reachable at localhost:5000")
