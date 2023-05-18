/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2023	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

using Skyline.DataMiner.Analytics.GenericInterface;
using System;

[GQIMetaData(Name = "Remaining Percentage")]
public class MyCustomOperator : IGQIColumnOperator, IGQIRowOperator, IGQIInputArguments
{
	private GQIColumnDropdownArgument _firstColumnArg = new GQIColumnDropdownArgument("Percentage") { IsRequired = true, Types = new GQIColumnType[] { GQIColumnType.Double } };
	private GQIStringArgument _nameArg = new GQIStringArgument("Column name") { IsRequired = true };

	private GQIColumn _firstColumn;
	private GQIDoubleColumn _newColumn;

	public GQIArgument[] GetInputArguments()
	{
		return new GQIArgument[] { _firstColumnArg, _nameArg };
	}

	public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
	{
		_firstColumn = args.GetArgumentValue(_firstColumnArg);
		_newColumn = new GQIDoubleColumn(args.GetArgumentValue(_nameArg));

		return new OnArgumentsProcessedOutputArgs();
	}

	public void HandleColumns(GQIEditableHeader header)
	{
		header.AddColumns(_newColumn);
	}

	public void HandleRow(GQIEditableRow row)
	{
		double dFirstValue;

		if (!row.TryGetValue(_firstColumn, out dFirstValue))
		{
			row.Delete();
			return;
		}

		if (dFirstValue < 0 || dFirstValue > 100)
		{
			// Negative %s not supported - what to do on this case?

			row.Delete();
			return;
		}
		else
		{
			var result = 100 - dFirstValue;
			var resultRounded = Math.Round(result, 2);
			row.SetValue(_newColumn, result, $"{ resultRounded}%");
		}
	}
}