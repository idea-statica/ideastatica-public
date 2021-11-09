﻿using Dlubal.RSTAB8;
using IdeaRS.OpenModel;
using IdeaStatiCa.Plugin;

namespace IdeaRstabPlugin
{
	internal class ImportSession : IImportSession
	{
		public CountryCode CountryCode { get; private set; }

		//public bool IsLCSOrientedUpwards { get; private set; }

		public bool IsGCSOrientedUpwards { get; private set; }

		public RequestedItemsType RequestedItemsType { get; private set; }

		private readonly IModel _model;

		public ImportSession(IModel model)
		{
			_model = model;
		}

		public void Setup(CountryCode countryCode, RequestedItemsType requestedItems)
		{
			CountryCode = countryCode;
			//IsLCSOrientedUpwards = _model.GetLocalZAxisOrientation() == OrientationType.Upward;
			IsGCSOrientedUpwards = _model.GetGlobalZAxisOrientation() == OrientationType.Upward;
			RequestedItemsType = requestedItems;
		}
	}
}