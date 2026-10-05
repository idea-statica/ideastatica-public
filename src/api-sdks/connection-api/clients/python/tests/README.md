# how to run pytest ?

Run from the directory _clients\python_, as `python -m pytest`: `python -m` puts that directory first on
`sys.path`, so the tests exercise the package in this repository even when a released
`ideastatica-connection-api` is installed. A bare `pytest` resolves the import to whatever is installed
instead, which is the way to test an installed wheel.

```
# Step 1: Create a virtual environment
python -m venv venv

# Step 2: Activate the virtual environment
.\venv\Scripts\activate  # On Windows

# Step 3: Install dependencies
pip install -r requirements.txt -r test-requirements.txt

# Step 4: Run the offline unit tests - what CI runs on every pull request
python -m pytest -m "not requires_service"

# Step 5: Run everything, with a Connection REST API service listening on http://localhost:5000
python -m pytest
```

A test that needs the running service goes into a module marked `pytestmark = pytest.mark.requires_service`
(the marker is registered in `tox.ini`); otherwise CI runs it without a service and it fails.