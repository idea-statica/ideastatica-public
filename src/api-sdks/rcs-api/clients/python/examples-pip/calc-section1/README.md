# calc-section1-attacher.py

This example shows how to create and attach _RcsApiClient_ to the running service _IdeaStatiCa.RcsRestApi_  

1. run _IdeaStatiCa.RcsRestApi.exe_ from IdeaStatiCa setup directory. Make sure the service is running by navigating to http://localhost:5000

2. navigate to the directory _examples-pip\calc-section1_

3. install dependencies from _requirements.txt_

```
pip install -r requirements.txt
```

4. run _calc-section1-attacher.py_. It createas and attaches RcsApiClient to the running _IdeaStatiCa.RcsRestApi_ on tcp/ip port 5000 (default port). The project _Project1.IdeaRcs_ is open calculated and results are printed.
