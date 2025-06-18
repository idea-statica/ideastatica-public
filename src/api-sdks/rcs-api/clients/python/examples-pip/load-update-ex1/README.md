# update_loading_using_service_attacher

This example shows how to update loading in the section and recalculate it.

Existing loding is provided in xml format by calling _get_section_loading_. Xml can be modified and updated values can be set by calling _set_section_loading_.

1. run _IdeaStatiCa.RcsRestApi.exe_ from IdeaStatiCa setup directory. Make sure the service is running by navigating to http://localhost:5000

2. navigate to the directory _examples-pip\load-update-ex1_

3. install dependencies from _requirements.txt_

```
pip install -r requirements.txt
```

4. run _update_loading_using_service_attacher.py_. It createas and attaches RcsApiClient to the running _IdeaStatiCa.RcsRestApi_ on tcp/ip port 5000 (default port). The project _Project1.IdeaRcs_ is open calculated and results are printed.

The example of xml with section extremes :

```xml
<?xml version="1.0" encoding="utf-16"?>
<ArrayOfSectionExtremeBase xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <SectionExtremeBase xsi:type="StandardSectionExtreme">
    <Id>1</Id>
    <Name />
    <Description>S 1 - E 1</Description>
    <Time>2419200</Time>
    <SectionBaseId>1</SectionBaseId>
    <Loads>
      <Id>-1</Id>
      <Name />
      <Loads>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>-50000</N>
            <Qy>0</Qy>
            <Qz>10000</Qz>
            <Mx>0</Mx>
            <My>20000</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>Fundamental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>Accidental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>FatigueResponse</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>FatigueCyclicResponse</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>-50000</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>20000</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>Characteristic</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>Frequent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>-50000</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>20000</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>QuasiPermanent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>FireResistance</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>Fundamental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>Fundamental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>Accidental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>Accidental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>Characteristic</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>Characteristic</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>Frequent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>Frequent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>QuasiPermanent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>QuasiPermanent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>FireResistance</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>FireResistance</CombiType>
          <Time>2419200</Time>
        </InputLoad>
      </Loads>
      <UseFundamental>true</UseFundamental>
      <UseAccidental>false</UseAccidental>
      <UseFatigue>true</UseFatigue>
      <UseCharacteristic>true</UseCharacteristic>
      <UseFrequent>true</UseFrequent>
      <UseQuasiPermanent>true</UseQuasiPermanent>
      <UsePermanentULS>true</UsePermanentULS>
      <UsePermanentSLS>true</UsePermanentSLS>
      <UseFireResistance>true</UseFireResistance>
      <CheckLimitationFundamental>NotDefined</CheckLimitationFundamental>
      <CheckLimitationAccidental>NotDefined</CheckLimitationAccidental>
      <CssItemsLoad />
      <ReinfBarsLoad />
      <TendonsLoad />
    </Loads>
  </SectionExtremeBase>
  <SectionExtremeBase xsi:type="StandardSectionExtreme">
    <Id>2</Id>
    <Name />
    <Description>S 1 - E 2</Description>
    <Time>2419200</Time>
    <SectionBaseId>1</SectionBaseId>
    <Loads>
      <Id>-1</Id>
      <Name />
      <Loads>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>30000</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>Fundamental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>Accidental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>FatigueResponse</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>FatigueCyclicResponse</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>30000</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>Characteristic</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>Frequent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>30000</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>QuasiPermanent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Total</LoadType>
          <CombiType>FireResistance</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>Fundamental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>Fundamental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>Accidental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>Accidental</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>Characteristic</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>Characteristic</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>Frequent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>Frequent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>QuasiPermanent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>QuasiPermanent</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>Begin</LoadType>
          <CombiType>FireResistance</CombiType>
          <Time>2419200</Time>
        </InputLoad>
        <InputLoad>
          <Id>-1</Id>
          <Name />
          <InternalForce>
            <N>0</N>
            <Qy>0</Qy>
            <Qz>0</Qz>
            <Mx>0</Mx>
            <My>0</My>
            <Mz>0</Mz>
          </InternalForce>
          <LoadType>End</LoadType>
          <CombiType>FireResistance</CombiType>
          <Time>2419200</Time>
        </InputLoad>
      </Loads>
      <UseFundamental>true</UseFundamental>
      <UseAccidental>false</UseAccidental>
      <UseFatigue>true</UseFatigue>
      <UseCharacteristic>true</UseCharacteristic>
      <UseFrequent>true</UseFrequent>
      <UseQuasiPermanent>true</UseQuasiPermanent>
      <UsePermanentULS>true</UsePermanentULS>
      <UsePermanentSLS>true</UsePermanentSLS>
      <UseFireResistance>true</UseFireResistance>
      <CheckLimitationFundamental>NotDefined</CheckLimitationFundamental>
      <CheckLimitationAccidental>NotDefined</CheckLimitationAccidental>
      <CssItemsLoad />
      <ReinfBarsLoad />
      <TendonsLoad />
    </Loads>
  </SectionExtremeBase>
</ArrayOfSectionExtremeBase>
```