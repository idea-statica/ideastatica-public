/*
 * Connection Rest API 1.0
 *
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * Generated by: https://github.com/openapitools/openapi-generator.git
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using OpenAPIDateConverter = IdeaStatiCa.ConnectionApi.Client.OpenAPIDateConverter;

namespace IdeaStatiCa.ConnectionApi.Model
{
    /// <summary>
    /// Provides data of the connected beam
    /// </summary>
    [DataContract(Name = "BeamData")]
    public partial class BeamData : IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeamData" /> class.
        /// </summary>
        /// <param name="name">Name of the beam.</param>
        /// <param name="plates">Plates of the beam.</param>
        /// <param name="crossSectionType">Type of cross section.</param>
        /// <param name="mprlName">MPRL name of beam.</param>
        /// <param name="originalModelId">Get or set the identification in the original model  In the case of the imported connection from another application.</param>
        /// <param name="cuts">Cuts on the beam.</param>
        /// <param name="isAdded">Is added beam.</param>
        /// <param name="addedMemberLength">Added beam lenght.</param>
        /// <param name="isNegativeObject">Is negative object.</param>
        /// <param name="addedMember">addedMember.</param>
        /// <param name="mirrorY">Mirror by Y.</param>
        /// <param name="refLineInCenterOfGravity">The reference line of the member is in the center of gravity of the cross-section.</param>
        /// <param name="isBearingMember">Is beam bearing member.</param>
        /// <param name="autoAddCutByWorkplane">Automaticali add cut by workplane if it not defined.</param>
        /// <param name="id">Element Id.</param>
        public BeamData(string name = default(string), List<PlateData> plates = default(List<PlateData>), string crossSectionType = default(string), string mprlName = default(string), string originalModelId = default(string), List<CutData> cuts = default(List<CutData>), bool isAdded = default(bool), double addedMemberLength = default(double), bool isNegativeObject = default(bool), ReferenceElement addedMember = default(ReferenceElement), bool mirrorY = default(bool), bool refLineInCenterOfGravity = default(bool), bool isBearingMember = default(bool), bool autoAddCutByWorkplane = default(bool), int id = default(int))
        {
            this.Name = name;
            this.Plates = plates;
            this.CrossSectionType = crossSectionType;
            this.MprlName = mprlName;
            this.OriginalModelId = originalModelId;
            this.Cuts = cuts;
            this.IsAdded = isAdded;
            this.AddedMemberLength = addedMemberLength;
            this.IsNegativeObject = isNegativeObject;
            this.AddedMember = addedMember;
            this.MirrorY = mirrorY;
            this.RefLineInCenterOfGravity = refLineInCenterOfGravity;
            this.IsBearingMember = isBearingMember;
            this.AutoAddCutByWorkplane = autoAddCutByWorkplane;
            this.Id = id;
        }

        /// <summary>
        /// Name of the beam
        /// </summary>
        /// <value>Name of the beam</value>
        [DataMember(Name = "name", EmitDefaultValue = true)]
        public string Name { get; set; }

        /// <summary>
        /// Plates of the beam
        /// </summary>
        /// <value>Plates of the beam</value>
        [DataMember(Name = "plates", EmitDefaultValue = true)]
        public List<PlateData> Plates { get; set; }

        /// <summary>
        /// Type of cross section
        /// </summary>
        /// <value>Type of cross section</value>
        [DataMember(Name = "crossSectionType", EmitDefaultValue = true)]
        public string CrossSectionType { get; set; }

        /// <summary>
        /// MPRL name of beam
        /// </summary>
        /// <value>MPRL name of beam</value>
        [DataMember(Name = "mprlName", EmitDefaultValue = true)]
        public string MprlName { get; set; }

        /// <summary>
        /// Get or set the identification in the original model  In the case of the imported connection from another application
        /// </summary>
        /// <value>Get or set the identification in the original model  In the case of the imported connection from another application</value>
        [DataMember(Name = "originalModelId", EmitDefaultValue = true)]
        public string OriginalModelId { get; set; }

        /// <summary>
        /// Cuts on the beam
        /// </summary>
        /// <value>Cuts on the beam</value>
        [DataMember(Name = "cuts", EmitDefaultValue = true)]
        public List<CutData> Cuts { get; set; }

        /// <summary>
        /// Is added beam
        /// </summary>
        /// <value>Is added beam</value>
        [DataMember(Name = "isAdded", EmitDefaultValue = true)]
        public bool IsAdded { get; set; }

        /// <summary>
        /// Added beam lenght
        /// </summary>
        /// <value>Added beam lenght</value>
        [DataMember(Name = "addedMemberLength", EmitDefaultValue = false)]
        public double AddedMemberLength { get; set; }

        /// <summary>
        /// Is negative object
        /// </summary>
        /// <value>Is negative object</value>
        [DataMember(Name = "isNegativeObject", EmitDefaultValue = true)]
        public bool IsNegativeObject { get; set; }

        /// <summary>
        /// Gets or Sets AddedMember
        /// </summary>
        [DataMember(Name = "addedMember", EmitDefaultValue = false)]
        public ReferenceElement AddedMember { get; set; }

        /// <summary>
        /// Mirror by Y
        /// </summary>
        /// <value>Mirror by Y</value>
        [DataMember(Name = "mirrorY", EmitDefaultValue = true)]
        public bool MirrorY { get; set; }

        /// <summary>
        /// The reference line of the member is in the center of gravity of the cross-section
        /// </summary>
        /// <value>The reference line of the member is in the center of gravity of the cross-section</value>
        [DataMember(Name = "refLineInCenterOfGravity", EmitDefaultValue = true)]
        public bool RefLineInCenterOfGravity { get; set; }

        /// <summary>
        /// Is beam bearing member
        /// </summary>
        /// <value>Is beam bearing member</value>
        [DataMember(Name = "isBearingMember", EmitDefaultValue = true)]
        public bool IsBearingMember { get; set; }

        /// <summary>
        /// Automaticali add cut by workplane if it not defined
        /// </summary>
        /// <value>Automaticali add cut by workplane if it not defined</value>
        [DataMember(Name = "autoAddCutByWorkplane", EmitDefaultValue = true)]
        public bool AutoAddCutByWorkplane { get; set; }

        /// <summary>
        /// Element Id
        /// </summary>
        /// <value>Element Id</value>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("class BeamData {\n");
            sb.Append("  Name: ").Append(Name).Append("\n");
            sb.Append("  Plates: ").Append(Plates).Append("\n");
            sb.Append("  CrossSectionType: ").Append(CrossSectionType).Append("\n");
            sb.Append("  MprlName: ").Append(MprlName).Append("\n");
            sb.Append("  OriginalModelId: ").Append(OriginalModelId).Append("\n");
            sb.Append("  Cuts: ").Append(Cuts).Append("\n");
            sb.Append("  IsAdded: ").Append(IsAdded).Append("\n");
            sb.Append("  AddedMemberLength: ").Append(AddedMemberLength).Append("\n");
            sb.Append("  IsNegativeObject: ").Append(IsNegativeObject).Append("\n");
            sb.Append("  AddedMember: ").Append(AddedMember).Append("\n");
            sb.Append("  MirrorY: ").Append(MirrorY).Append("\n");
            sb.Append("  RefLineInCenterOfGravity: ").Append(RefLineInCenterOfGravity).Append("\n");
            sb.Append("  IsBearingMember: ").Append(IsBearingMember).Append("\n");
            sb.Append("  AutoAddCutByWorkplane: ").Append(AutoAddCutByWorkplane).Append("\n");
            sb.Append("  Id: ").Append(Id).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}