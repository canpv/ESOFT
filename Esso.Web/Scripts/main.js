
function exportTo(type) {

	$('#table').tableExport({
		filename: 'table_%DD%-%MM%-%YY%',
		format: type,
		cols: '2,3,4'
	});

	$('#tableInv').tableExport({
		filename: 'table_%DD%-%MM%-%YY%',
		format: type,
		cols: '2,3,4'
	});


}

function exportAll(type, name) {
	var _datepicker = $('#datepicker').val();
	var _datepicker2 = $('#datepicker2').val();
	if (name == "csvEkk" || name == "xlsEkk" || name == "txtEkk") {

		if (_datepicker != _datepicker2 ) {
			$('#table').tableExport({
				filename: 'EKK-METEOROLOGY (' + _datepicker + ' / ' + _datepicker2 + ')',
				format: type
			});
		} else {
			$('#table').tableExport({
				filename: 'EKK-METEOROLOGY (' + _datepicker + ')',
				format: type
			});
		}
		

	} else if (name == "csvInv" || name == "xlsInv" || name == "txtInv") {
		if (_datepicker != _datepicker2) {
			$('#tableInv').tableExport({
				filename: 'INVERTER (' + _datepicker + ' / ' + _datepicker2 + ')',
				format: type
			});
		} else {
			$('#tableInv').tableExport({
				filename: 'INVERTER (' + _datepicker +')',
				format: type
			});
		}
	} 

	
}
