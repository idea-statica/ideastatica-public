# coding: utf-8

"""
    Connection Rest API 2.0

    IDEA StatiCa Connection API, used for the automated design and calculation of steel connections.

    The version of the OpenAPI document: 2.0
    Contact: info@ideastatica.com
    Generated by OpenAPI Generator (https://openapi-generator.tech)

    Do not edit the class manually.
"""  # noqa: E501


from __future__ import annotations
import pprint
import re  # noqa: F401
import json

from pydantic import BaseModel, ConfigDict, Field
from typing import Any, ClassVar, Dict, List, Optional
from ideastatica_connection_api.models.anchor_grid import AnchorGrid
from ideastatica_connection_api.models.beam_data import BeamData
from ideastatica_connection_api.models.bolt_grid import BoltGrid
from ideastatica_connection_api.models.concrete_block_data import ConcreteBlockData
from ideastatica_connection_api.models.cut_beam_by_beam_data import CutBeamByBeamData
from ideastatica_connection_api.models.folded_plate_data import FoldedPlateData
from ideastatica_connection_api.models.pin_grid import PinGrid
from ideastatica_connection_api.models.plate_data import PlateData
from ideastatica_connection_api.models.reference_element import ReferenceElement
from ideastatica_connection_api.models.weld_data import WeldData
from typing import Optional, Set
from typing_extensions import Self

class ConnectionData(BaseModel):
    """
    Provides data of the connection
    """ # noqa: E501
    connection_point: Optional[ReferenceElement] = Field(default=None, alias="connectionPoint")
    beams: Optional[List[BeamData]] = Field(default=None, description="Connected beams")
    plates: Optional[List[PlateData]] = Field(default=None, description="Plates of the connection")
    folded_plates: Optional[List[FoldedPlateData]] = Field(default=None, description="Folded plate of the connection", alias="foldedPlates")
    bolt_grids: Optional[List[BoltGrid]] = Field(default=None, description="Bolt grids which belongs to the connection", alias="boltGrids")
    anchor_grids: Optional[List[AnchorGrid]] = Field(default=None, description="Anchor grids which belongs to the connection", alias="anchorGrids")
    pin_grids: Optional[List[PinGrid]] = Field(default=None, description="Pin grids which belongs to the connection", alias="pinGrids")
    welds: Optional[List[WeldData]] = Field(default=None, description="Welds of the connection")
    concrete_blocks: Optional[List[ConcreteBlockData]] = Field(default=None, description="ConcreteBlocksof the connection", alias="concreteBlocks")
    cut_beam_by_beams: Optional[List[CutBeamByBeamData]] = Field(default=None, description="cut beam by beams", alias="cutBeamByBeams")
    __properties: ClassVar[List[str]] = ["connectionPoint", "beams", "plates", "foldedPlates", "boltGrids", "anchorGrids", "pinGrids", "welds", "concreteBlocks", "cutBeamByBeams"]

    model_config = ConfigDict(
        populate_by_name=True,
        validate_assignment=True,
        protected_namespaces=(),
    )


    def to_str(self) -> str:
        """Returns the string representation of the model using alias"""
        return pprint.pformat(self.model_dump(by_alias=True))

    def to_json(self) -> str:
        """Returns the JSON representation of the model using alias"""
        # TODO: pydantic v2: use .model_dump_json(by_alias=True, exclude_unset=True) instead
        return json.dumps(self.to_dict())

    @classmethod
    def from_json(cls, json_str: str) -> Optional[Self]:
        """Create an instance of ConnectionData from a JSON string"""
        return cls.from_dict(json.loads(json_str))

    def to_dict(self) -> Dict[str, Any]:
        """Return the dictionary representation of the model using alias.

        This has the following differences from calling pydantic's
        `self.model_dump(by_alias=True)`:

        * `None` is only added to the output dict for nullable fields that
          were set at model initialization. Other fields with value `None`
          are ignored.
        """
        excluded_fields: Set[str] = set([
        ])

        _dict = self.model_dump(
            by_alias=True,
            exclude=excluded_fields,
            exclude_none=True,
        )
        # override the default output from pydantic by calling `to_dict()` of connection_point
        if self.connection_point:
            _dict['connectionPoint'] = self.connection_point.to_dict()
        # override the default output from pydantic by calling `to_dict()` of each item in beams (list)
        _items = []
        if self.beams:
            for _item_beams in self.beams:
                if _item_beams:
                    _items.append(_item_beams.to_dict())
            _dict['beams'] = _items
        # override the default output from pydantic by calling `to_dict()` of each item in plates (list)
        _items = []
        if self.plates:
            for _item_plates in self.plates:
                if _item_plates:
                    _items.append(_item_plates.to_dict())
            _dict['plates'] = _items
        # override the default output from pydantic by calling `to_dict()` of each item in folded_plates (list)
        _items = []
        if self.folded_plates:
            for _item_folded_plates in self.folded_plates:
                if _item_folded_plates:
                    _items.append(_item_folded_plates.to_dict())
            _dict['foldedPlates'] = _items
        # override the default output from pydantic by calling `to_dict()` of each item in bolt_grids (list)
        _items = []
        if self.bolt_grids:
            for _item_bolt_grids in self.bolt_grids:
                if _item_bolt_grids:
                    _items.append(_item_bolt_grids.to_dict())
            _dict['boltGrids'] = _items
        # override the default output from pydantic by calling `to_dict()` of each item in anchor_grids (list)
        _items = []
        if self.anchor_grids:
            for _item_anchor_grids in self.anchor_grids:
                if _item_anchor_grids:
                    _items.append(_item_anchor_grids.to_dict())
            _dict['anchorGrids'] = _items
        # override the default output from pydantic by calling `to_dict()` of each item in pin_grids (list)
        _items = []
        if self.pin_grids:
            for _item_pin_grids in self.pin_grids:
                if _item_pin_grids:
                    _items.append(_item_pin_grids.to_dict())
            _dict['pinGrids'] = _items
        # override the default output from pydantic by calling `to_dict()` of each item in welds (list)
        _items = []
        if self.welds:
            for _item_welds in self.welds:
                if _item_welds:
                    _items.append(_item_welds.to_dict())
            _dict['welds'] = _items
        # override the default output from pydantic by calling `to_dict()` of each item in concrete_blocks (list)
        _items = []
        if self.concrete_blocks:
            for _item_concrete_blocks in self.concrete_blocks:
                if _item_concrete_blocks:
                    _items.append(_item_concrete_blocks.to_dict())
            _dict['concreteBlocks'] = _items
        # override the default output from pydantic by calling `to_dict()` of each item in cut_beam_by_beams (list)
        _items = []
        if self.cut_beam_by_beams:
            for _item_cut_beam_by_beams in self.cut_beam_by_beams:
                if _item_cut_beam_by_beams:
                    _items.append(_item_cut_beam_by_beams.to_dict())
            _dict['cutBeamByBeams'] = _items
        # set to None if beams (nullable) is None
        # and model_fields_set contains the field
        if self.beams is None and "beams" in self.model_fields_set:
            _dict['beams'] = None

        # set to None if plates (nullable) is None
        # and model_fields_set contains the field
        if self.plates is None and "plates" in self.model_fields_set:
            _dict['plates'] = None

        # set to None if folded_plates (nullable) is None
        # and model_fields_set contains the field
        if self.folded_plates is None and "folded_plates" in self.model_fields_set:
            _dict['foldedPlates'] = None

        # set to None if bolt_grids (nullable) is None
        # and model_fields_set contains the field
        if self.bolt_grids is None and "bolt_grids" in self.model_fields_set:
            _dict['boltGrids'] = None

        # set to None if anchor_grids (nullable) is None
        # and model_fields_set contains the field
        if self.anchor_grids is None and "anchor_grids" in self.model_fields_set:
            _dict['anchorGrids'] = None

        # set to None if pin_grids (nullable) is None
        # and model_fields_set contains the field
        if self.pin_grids is None and "pin_grids" in self.model_fields_set:
            _dict['pinGrids'] = None

        # set to None if welds (nullable) is None
        # and model_fields_set contains the field
        if self.welds is None and "welds" in self.model_fields_set:
            _dict['welds'] = None

        # set to None if concrete_blocks (nullable) is None
        # and model_fields_set contains the field
        if self.concrete_blocks is None and "concrete_blocks" in self.model_fields_set:
            _dict['concreteBlocks'] = None

        # set to None if cut_beam_by_beams (nullable) is None
        # and model_fields_set contains the field
        if self.cut_beam_by_beams is None and "cut_beam_by_beams" in self.model_fields_set:
            _dict['cutBeamByBeams'] = None

        return _dict

    @classmethod
    def from_dict(cls, obj: Optional[Dict[str, Any]]) -> Optional[Self]:
        """Create an instance of ConnectionData from a dict"""
        if obj is None:
            return None

        if not isinstance(obj, dict):
            return cls.model_validate(obj)

        _obj = cls.model_validate({
            "connectionPoint": ReferenceElement.from_dict(obj["connectionPoint"]) if obj.get("connectionPoint") is not None else None,
            "beams": [BeamData.from_dict(_item) for _item in obj["beams"]] if obj.get("beams") is not None else None,
            "plates": [PlateData.from_dict(_item) for _item in obj["plates"]] if obj.get("plates") is not None else None,
            "foldedPlates": [FoldedPlateData.from_dict(_item) for _item in obj["foldedPlates"]] if obj.get("foldedPlates") is not None else None,
            "boltGrids": [BoltGrid.from_dict(_item) for _item in obj["boltGrids"]] if obj.get("boltGrids") is not None else None,
            "anchorGrids": [AnchorGrid.from_dict(_item) for _item in obj["anchorGrids"]] if obj.get("anchorGrids") is not None else None,
            "pinGrids": [PinGrid.from_dict(_item) for _item in obj["pinGrids"]] if obj.get("pinGrids") is not None else None,
            "welds": [WeldData.from_dict(_item) for _item in obj["welds"]] if obj.get("welds") is not None else None,
            "concreteBlocks": [ConcreteBlockData.from_dict(_item) for _item in obj["concreteBlocks"]] if obj.get("concreteBlocks") is not None else None,
            "cutBeamByBeams": [CutBeamByBeamData.from_dict(_item) for _item in obj["cutBeamByBeams"]] if obj.get("cutBeamByBeams") is not None else None
        })
        return _obj


