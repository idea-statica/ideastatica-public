# import-iom-ex1-attacher

This example shows how to create and attach _RcsApiClient_ to the running service _IdeaStatiCa.RcsRestApi_  

1. run _IdeaStatiCa.RcsRestApi.exe_ from IdeaStatiCa setup directory. Make sure the service is running by navigating to http://localhost:5000

2. navigate to the directory _examples-pip\import-iom-ex1_

3. Install dependencies from _requirements.txt_

``` 
pip install -r requirements.txt
```

4. Make sure that your path to IdeaStatiCa.RcsRestApi.exe file is correct

5. Run _import-iom-ex1-attacher.py_. It creates and attaches _RcsApiClient_ to the service runned by service runner. The project _Project1.IdeaRcs_ is open calculated and results are printed.