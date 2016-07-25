import React from 'react';
import Table from 'react-bootstrap/lib/Table';
import OverlayTrigger from 'react-bootstrap/lib/OverlayTrigger';
import Popover from 'react-bootstrap/lib/Popover';
import Button from 'react-bootstrap/lib/Button';

class App extends React.Component {
	constructor() {
	  super();
	  
	  this.state = {
         data: {"club":{"id":1,"name":"Kelab Darul Ehsan"},"flights":[{"minPax":2,"maxPax":4,"minCart":0,"maxCart":2,"minCaddy":0,"maxCaddy":2,"tee_time":"07:00","booked":1,"matrix_id":1,"reserve_status":null,"prices":{"flight":"90.0","cart":"100.0","caddy":"90.0","insurance":"90.0","note":"","insurance_mode":1}},{"minPax":2,"maxPax":4,"minCart":0,"maxCart":2,"minCaddy":0,"maxCaddy":2,"tee_time":"07:15","booked":null,"matrix_id":2,"reserve_status":1,"prices":{"flight":"90.0","cart":"100.0","caddy":"90.0","insurance":"90.0","note":"","insurance_mode":1}},{"minPax":2,"maxPax":4,"minCart":0,"maxCart":2,"minCaddy":0,"maxCaddy":2,"tee_time":"07:30","booked":null,"matrix_id":3,"reserve_status":null,"prices":{"flight":"90.0","cart":"100.0","caddy":"90.0","insurance":"90.0","note":"","insurance_mode":1}},{"minPax":2,"maxPax":4,"minCart":0,"maxCart":2,"minCaddy":0,"maxCaddy":2,"tee_time":"07:45","booked":null,"matrix_id":4,"reserve_status":null,"prices":{"flight":"90.0","cart":"100.0","caddy":"90.0","insurance":"90.0","note":"","insurance_mode":1}},{"minPax":2,"maxPax":4,"minCart":0,"maxCart":2,"minCaddy":0,"maxCaddy":2,"tee_time":"08:00","booked":null,"matrix_id":5,"reserve_status":null,"prices":{"flight":"90.0","cart":"100.0","caddy":"90.0","insurance":"90.0","note":"","insurance_mode":1}},{"minPax":2,"maxPax":4,"minCart":0,"maxCart":2,"minCaddy":0,"maxCaddy":2,"tee_time":"08:30","booked":null,"matrix_id":6,"reserve_status":null,"prices":{"flight":"90.0","cart":"100.0","caddy":"90.0","insurance":"90.0","note":"","insurance_mode":1}},{"minPax":2,"maxPax":4,"minCart":0,"maxCart":2,"minCaddy":0,"maxCaddy":2,"tee_time":"09:00","booked":null,"matrix_id":7,"reserve_status":null,"prices":{"flight":"90.0","cart":"100.0","caddy":"90.0","insurance":"90.0","note":"","insurance_mode":1}},{"minPax":3,"maxPax":4,"minCart":1,"maxCart":2,"minCaddy":1,"maxCaddy":2,"tee_time":"16:00","booked":null,"matrix_id":14,"reserve_status":null,"prices":{"flight":"180.0","cart":"100.0","caddy":"200.0","insurance":"50.0","note":"Free:-\r\n* drinks","insurance_mode":1}},{"minPax":3,"maxPax":4,"minCart":1,"maxCart":2,"minCaddy":1,"maxCaddy":2,"tee_time":"16:30","booked":null,"matrix_id":15,"reserve_status":null,"prices":{"flight":"180.0","cart":"100.0","caddy":"200.0","insurance":"50.0","note":"Free:-\r\n* drinks","insurance_mode":1}},{"minPax":3,"maxPax":4,"minCart":1,"maxCart":2,"minCaddy":1,"maxCaddy":2,"tee_time":"17:00","booked":null,"matrix_id":16,"reserve_status":null,"prices":{"flight":"180.0","cart":"100.0","caddy":"200.0","insurance":"50.0","note":"Free:-\r\n* drinks","insurance_mode":1}},{"minPax":3,"maxPax":4,"minCart":1,"maxCart":2,"minCaddy":1,"maxCaddy":2,"tee_time":"17:30","booked":null,"matrix_id":17,"reserve_status":null,"prices":{"flight":"180.0","cart":"100.0","caddy":"200.0","insurance":"50.0","note":"Free:-\r\n* drinks","insurance_mode":1}},{"minPax":3,"maxPax":4,"minCart":1,"maxCart":2,"minCaddy":1,"maxCaddy":2,"tee_time":"18:00","booked":null,"matrix_id":18,"reserve_status":null,"prices":{"flight":"180.0","cart":"100.0","caddy":"200.0","insurance":"50.0","note":"Free:-\r\n* drinks","insurance_mode":1}},{"minPax":3,"maxPax":4,"minCart":1,"maxCart":2,"minCaddy":1,"maxCaddy":2,"tee_time":"18:30","booked":null,"matrix_id":19,"reserve_status":null,"prices":{"flight":"180.0","cart":"100.0","caddy":"200.0","insurance":"50.0","note":"Free:-\r\n* drinks","insurance_mode":1}},{"minPax":3,"maxPax":4,"minCart":1,"maxCart":2,"minCaddy":1,"maxCaddy":2,"tee_time":"19:00","booked":null,"matrix_id":20,"reserve_status":null,"prices":{"flight":"180.0","cart":"100.0","caddy":"200.0","insurance":"50.0","note":"Free:-\r\n* drinks","insurance_mode":1}}],"queryData":{"date":"19/07/2016","query":""}}
      }
	}

	render() {	  
	  return (
		 <div>
			<Table striped bordered condensed>
				<thead>
					<tr>
						<th></th>
						{this.state.data.flights.map((flight, i) => <TableHeadColumn key = {i} data = {flight} />)}
					</tr>
				</thead>
				<tbody>
					<tr>
						<TableRowHead headerProp = {this.state.data.queryData.date} />
						{this.state.data.flights.map((flight, i) => <TableRowColumn key = {i} data = {flight} />)}
					</tr>
				</tbody>
			</Table>
		 </div>
	  );
	}
}

class TableHeadColumn extends React.Component {
   render() {
	  var myStyle = {
         textAlign:'center'
      }
      return (
         <th style={myStyle}>
            {this.props.data.tee_time}
         </th>
      );
   }
}

class TableRowHead extends React.Component {
   render() {
	  var myStyle = {
         textAlign:'center'
      }
      return (
         <th style={myStyle}>
            {this.props.headerProp}
         </th>
      );
   }
}

class TableRowColumn extends React.Component {
   render() {
      var myStyle = {
         background: '#FFFFFF',
		 padding: 0
      }
	  
	  if(this.props.data.booked != null) {
		  myStyle = {
			 background: '#FF0000',
			 padding: 0
		  }
	  }
	  else if(this.props.data.reserve_status != null) {
		  myStyle = {
			 background: '#FFFF00',
			 padding: 0
		  }
	  }
	  
      return (
         <td style={myStyle}>
            <OverlayTrigger trigger="click" rootClose placement="bottom" overlay={<Popover title="Popover bottom"><strong>Holy guacamole!</strong> Check this info.</Popover>}>
			  <a role='button' />
			</OverlayTrigger>
         </td>
      );
   }
}

export default App;