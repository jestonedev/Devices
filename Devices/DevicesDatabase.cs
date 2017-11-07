using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Devices
{
	/// <summary>
	/// Класс информации об узле
	/// </summary>
	[Serializable()]
	internal class Node: IComparable<Node>
	{
		public string NodeName { get; set; }
		public int NodeId { get; set; }
		public int ParentNodeId { get; set; }

		public Node(string nodeName, int nodeId, int parentNodeId)
		{
			NodeId = nodeId;
			NodeName = nodeName;
			ParentNodeId = parentNodeId;
		}

		#region IComparable<Node> Members

		public int CompareTo(Node other)
		{
			return string.Compare(NodeName, other.NodeName, StringComparison.Ordinal);
		}

		#endregion
	}

	internal class DevicesDatabase: IDisposable
	{
		private readonly SqlConnection _connection;

		public DevicesDatabase()
		{
			_connection = new SqlConnection(Properties.Settings.Default.DevicesConnectionString);
			try
			{
				_connection.Open();
			}
			catch
			{
				MessageBox.Show(@"Ошибка соединения с сервером базы данных. Проверьте правильность строки соединения в файле конфигурации и доступность сервера", 
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}		

		public void  Dispose()
		{
			_connection.Close();
		}
        
        /// <summary>
        /// Возвращает список типов периферийного оборудования
        /// </summary>
        public List<PeripheryType> GetPeripheryType()
        {
            if (_connection.State != ConnectionState.Open)
                return new List<PeripheryType>();
            var command =
                new SqlCommand(@"SELECT cv.[ID Value],cv.Value FROM dbo.ComboboxValues cv WHERE cv.[ID Node]=25")
                {
                    Connection = _connection
                };
            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить список периферийного оборудования. " + e.Message, @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<PeripheryType>();
            }
            var peripheryType = new List<PeripheryType>();            
            while(reader.Read())
            {
                var id = reader.GetInt32(0);
                var value = reader.GetString(1);
                peripheryType.Add(new PeripheryType { Id = id, Value = value });
            }
            reader.Close();
            return peripheryType;
        }
		/// <summary>
		/// Возвращает список всех департаментов
		/// </summary>
		public List<Node> GetDepartments(SearchParametersGroup spg)
		{
			if (_connection.State != ConnectionState.Open)
				return new List<Node>();
			var where = "";
			if (spg.DepartmentIDs.Count > 0)
			{
				where = "[ID Department] IN (";
				foreach (var departmentId in spg.DepartmentIDs)
				{
					where += departmentId + ",";
				}
				where = where.Trim(',');
				where += ")";
			}

			if (where.Trim().Length > 0)
				where = "WHERE " + where;
		    var command = new SqlCommand(@"SELECT [ID Department], [ID Parent Department], Department
				FROM dbo.Departments " + where + " ORDER BY [ID Parent Department]")
		    {
		        Connection = _connection
		    };
		    SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить список департаментов. "+e.Message, @"Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var department = reader.GetString(2);
				var departmentId = reader.GetInt32(0);
				int parentId;
				try {
					parentId = reader.GetInt32(1);
				}
				catch {
					parentId = 0;
				}
				list.Add(new Node(department, departmentId, parentId));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Возвращает список всего сетевого оборудования
		/// </summary>
		public List<Node> GetDevices(SearchParametersGroup spg)
		{
			if (_connection.State != ConnectionState.Open)
				return new List<Node>();
			var where = "";
			if (spg.DeviceName.Trim().Length > 0)
			{
                where += "([Device Name] LIKE '%" + spg.DeviceName.Trim() + "%')";
			}
            if (spg.SerialNumber.Trim().Length > 0)
            {
                if (where.Trim().Length > 0)
                    where += " AND ";
                where += "([SerialNumber] LIKE '%" + spg.SerialNumber.Trim() + "%')";
            }
            if (spg.InventoryNumber.Trim().Length > 0)
            {
                if (where.Trim().Length > 0)
                    where += " AND ";
                where += "([InventoryNumber] LIKE '%" + spg.InventoryNumber.Trim() + "%')";
            }
            if (spg.DeviceTypeId > 0)
		    {
                if (where.Trim().Length > 0)
                    where += " AND ";
                where += "([ID Device Type] = " + spg.DeviceTypeId + ")";
		    }
		    if (spg.DepartmentIDs.Count > 0)
			{
				if (where.Trim().Length > 0)
					where += " AND ";
				where += "[ID Department] IN (";
				foreach (var departmentId in spg.DepartmentIDs)
				{
					where += departmentId + ",";
				}
				where = where.Trim(',');
				where += ")";
			}
			BuildDeviceFilterByNodeParameters(spg, ref where);
			BuildDeviceFilterByMonitoringParameters(spg, ref where);	    
			if (where.Trim().Length > 0)
				where = "WHERE " + where;
		    var command = new SqlCommand(@"SELECT [ID Device], [ID Department], [Device Name]
												FROM Devices " + where) {Connection = _connection};
		    SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить список оборудования. "+e.Message, @"Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var device = reader.GetString(2);
				var deviceId = reader.GetInt32(0);
				int departmentId;
				try {
					departmentId = reader.GetInt32(1);
				}
				catch {
					departmentId = 0;
				}
				list.Add(new Node(device, deviceId, departmentId));
			}
			reader.Close();
			return list;
		}

        private void BuildDeviceFilterByMonitoringParameters(SearchParametersGroup spg, ref string where)
        {
            if (spg.MonitoringParameters.Count <= 0) return;
            var monitoringProperties = GetDeviceAllMonitoringPropertiesInfo();
            var deviceIds = new List<int>();
            foreach (var searchMonitoringParameter in spg.MonitoringParameters)
            {
                foreach (DataRowView monitoringProperty in monitoringProperties)
                {
                    if (monitoringProperty.Row["Property Name"] == DBNull.Value) continue;
                    if (searchMonitoringParameter.ParameterName != (string) monitoringProperty.Row["Property Name"])
                        continue;

                    double monitoringPropertyParsed, searchMonitoringParameterParsed = 0;
                    var doubleCorrect = 
                        double.TryParse((string) monitoringProperty.Row["Property Value"], out monitoringPropertyParsed) &&
                        double.TryParse(searchMonitoringParameter.ParameterValue, out searchMonitoringParameterParsed);
                    switch (searchMonitoringParameter.Operation)
                    {
                        case "=":
                            if ((string) monitoringProperty.Row["Property Value"] ==
                                searchMonitoringParameter.ParameterValue)
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                        case "<>":
                            if ((string)monitoringProperty.Row["Property Value"] !=
                                searchMonitoringParameter.ParameterValue)
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                        case ">":
                            if (doubleCorrect && monitoringPropertyParsed > searchMonitoringParameterParsed)
                            {
                                    deviceIds.Add((int)monitoringProperty.Row["ID Device"]);   
                            }
                            break;
                        case ">=":
                            if (doubleCorrect && monitoringPropertyParsed >= searchMonitoringParameterParsed)
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                        case "<":
                            if (doubleCorrect && monitoringPropertyParsed < searchMonitoringParameterParsed)
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                        case "<=":
                            if (doubleCorrect && monitoringPropertyParsed <= searchMonitoringParameterParsed)
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                        case "Содержит":
                            if (((string)monitoringProperty.Row["Property Value"]).Contains(searchMonitoringParameter.ParameterValue))
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                        case "Не содержит":
                            if (!((string)monitoringProperty.Row["Property Value"]).Contains(searchMonitoringParameter.ParameterValue))
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                        case "Начинается с":
                            if (((string)monitoringProperty.Row["Property Value"]).StartsWith(searchMonitoringParameter.ParameterValue))
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                        case "Начинается не с":
                            if (!((string)monitoringProperty.Row["Property Value"]).StartsWith(searchMonitoringParameter.ParameterValue))
                            {
                                deviceIds.Add((int)monitoringProperty.Row["ID Device"]);
                            }
                            break;
                    }
                }
                if (where.Trim().Length > 0 && deviceIds.Count > 0)
                    where += " AND [ID Device] IN (";
                where += deviceIds.Select(r => r.ToString()).Aggregate((acc, v) => acc + "," + v);
                where += ")";
                deviceIds.Clear();
            }
        }

	    private static void BuildDeviceFilterByNodeParameters(SearchParametersGroup spg, ref string where)
	    {
	        if (spg.NodeParameters.Count <= 0) return;
	        if (where.Trim().Length > 0)
	            where += " AND ";
	        where += @"[ID Device] IN
						(select [ID Device]
						from dbo.Nodes
						where ";
	        var i = 0;
	        foreach (var sp in spg.NodeParameters)
	        {
	            where += "([ID AssocMetaNode] = " + sp.ParameterId + ") AND ";
	            switch (sp.ParameterType)
	            {
	                case "int":
	                    where += "(CAST([Value] AS INT) " + sp.Operation + " " + "CAST(" + sp.ParameterValue + " AS INT))";
	                    break;
	                case "float":
	                    where += "(CAST(REPLACE([Value],',','.') AS FLOAT) " + sp.Operation + " " + "(CAST(REPLACE(" +
	                             sp.ParameterValue + ",',','.') AS FLOAT)))";
	                    break;
	                default:
	                    where += "([Value] " + sp.Operation + " " + sp.ParameterValue + ")";
	                    break;
	            }
	            i++;
	            if (i != spg.NodeParameters.Count)
	                where += " OR ";
	        }
	        where += " group by [ID Device] having COUNT(*) >= " + spg.NodeParameters.Count + ")";
	    }

	    /// <summary>
	    /// Возвращает список всех характеристик устройств по типу устройства
	    /// </summary>
	    /// <param name="deviceTypeId"></param>
	    /// <returns></returns>
	    public List<Node> GetDeviceInfoMetaByType(int deviceTypeId)
		{
			if (_connection.State != ConnectionState.Open)
				return new List<Node>();
	        var command = new SqlCommand(@"SELECT [ID Node], [ID Parent Node], [Parameter Name]
				FROM NodeMeta
				WHERE [Parameter Type] = 'complex' AND [ID Device Type] = @DeviceTypeID
				ORDER BY [Order], [ID Parent Node]") {Connection = _connection};
	        command.Parameters.Add(new SqlParameter("DeviceTypeID", deviceTypeId));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить мета-данные структуры. " + e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var parameterName = reader.GetString(2);
				var nodeId = reader.GetInt32(0);
				int parentNodeId;
				try
				{
					parentNodeId = reader.GetInt32(1);
				}
				catch
				{
					parentNodeId = 0;
				}
				list.Add(new Node(parameterName, nodeId, parentNodeId));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Возвращает список всех характеристик устройства
		/// </summary>
		public List<Node> GetDeviceInfoMeta(int deviceId)
		{
			if (_connection.State != ConnectionState.Open)
				return new List<Node>();
		    var command = new SqlCommand(@"SELECT [ID Node], [ID Parent Node], [Parameter Name]
				FROM NodeMeta
				WHERE [Parameter Type] = 'complex' AND [ID Device Type] = (SELECT [ID Device Type]
				FROM Devices
				WHERE [ID Device] = @DeviceID)
				ORDER BY [Order], [ID Parent Node]") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("DeviceID",deviceId));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить мета-данные структуры. "+e.Message, @"Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var parameterName = reader.GetString(2);
				var nodeId = reader.GetInt32(0);
				int parentNodeId;
				try
				{
					parentNodeId = reader.GetInt32(1);
				}
				catch
				{
					parentNodeId = 0;
				}
				list.Add(new Node(parameterName, nodeId, parentNodeId));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Получить общую информацию об устройстве
		/// </summary>
		/// <param name="deviceId">Идентификатор устройства</param>
		/// <returns></returns>
		public List<Node> GetDeviceInfo(int deviceId)
		{
			if (_connection.State != ConnectionState.Open)
				return new List<Node>();
		    var command = new SqlCommand(@"SELECT Nodes.[ID Node], NodeMeta.[ID Node], [Value]
					FROM dbo.NodeMeta LEFT JOIN dbo.Nodes ON (NodeMeta.[ID Node] = Nodes.[ID AssocMetaNode])
					WHERE ([ID Device] = @DeviceID) AND ([Parameter Type] = 'complex')
					ORDER BY NodeMeta.[ID Node]") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию о характеристиках компьютера. "+e.Message, @"Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var value = reader.GetString(2);
				var nodeId = reader.GetInt32(0);
				int parentNodeId;
				try
				{
					parentNodeId = reader.GetInt32(1);
				}
				catch
				{
					parentNodeId = 0;
				}
				list.Add(new Node(value, nodeId, parentNodeId));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Получить параметры устройства
		/// </summary>
		/// <returns></returns>
		public List<DeviceParametersComboboxItem> GetDeviceParameters(int parentNodeId)
		{
			if (_connection.State != ConnectionState.Open)
				return new List<DeviceParametersComboboxItem>();
		    var command = new SqlCommand(@"Select [ID Node], [Parameter Name], [Parameter Type]
				from dbo.NodeMeta
				where [Parameter Type] <> 'complex' AND [ID Parent Node] = @ParentNodeID
				order by [Order]") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("ParentNodeID", parentNodeId));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию о характеристиках компьютера. " + e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<DeviceParametersComboboxItem>();
			}
			var list = new List<DeviceParametersComboboxItem>();
			while (reader.Read())
			{
				var parameterName = reader.GetString(1);
				var parameterType = reader.GetString(2);
				var nodeId = reader.GetInt32(0);
				list.Add(new DeviceParametersComboboxItem(nodeId, parameterName, parameterType));
			}
			reader.Close();
			return list;
		}

	    /// <summary>
	    /// Получить детальную информацию об отдельном узле устройства
	    /// </summary>
	    /// <param name="deviceNodeId"></param>
	    /// <returns>Возвращает информацию об узле в табличном виде</returns>
	    public DataView GetDetailDeviceInfo(int deviceNodeId)
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
	        var command =
	            new SqlCommand(@"SELECT b.[ID Node] AS NodeRealID, a.[ID Node], b.[ID Parent Node], a.[Parameter Name], 
						a.[Parameter Type], b.Value
				FROM
				(SELECT *
				FROM dbo.NodeMeta
				WHERE dbo.NodeMeta.[ID Parent Node] = (SELECT [ID AssocMetaNode] 
						FROM Nodes WHERE [ID Node] = @DeviceNodeID)) a
				LEFT JOIN
				(SELECT *
				FROM Nodes
				WHERE [ID Parent Node] = @DeviceNodeID) b ON (a.[ID Node] = b.[ID AssocMetaNode])") {Connection = _connection};
	        command.Parameters.Add(new SqlParameter("DeviceNodeID", deviceNodeId));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch(SqlException e)
			{
				MessageBox.Show(@"Не удалось получить детальную информацию об узле устройства. "+e.Message, @"Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Получить список известных типов устройств
		/// </summary>
		public DataView GetDeviceTypes()
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
		    var command = new SqlCommand(@"SELECT * FROM [Device Types]") {Connection = _connection};
		    var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить список известных типов устройств. "+e.Message, @"Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Добавить департамент в базу данных
		/// </summary>
		/// <returns>Возвращает ID добавленной ноды при успешной операции и -1 при неудаче</returns>
		public int InsertDepartment(string department, int parentDepartmentId)
		{
		    var command =
		        new SqlCommand(
		            @"INSERT INTO Departments([ID Parent Department], [Department]) VALUES (@ParentDepartmentID, @Department);
					SET @INSERTED_ID=SCOPE_IDENTITY();") {Connection = _connection};
		    var insertedId = new SqlParameter
		    {
		        ParameterName = "INSERTED_ID",
		        Direction = ParameterDirection.Output,
		        Size = 4
		    };
		    command.Parameters.Add(insertedId);
			command.Parameters.Add(new SqlParameter("ParentDepartmentID", parentDepartmentId));
			command.Parameters.Add(new SqlParameter("Department", department));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на добавление департамента", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось добавить департамент."+e.Message, @"Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return -1;
			}
			return int.Parse(insertedId.Value.ToString());
		}

		/// <summary>
		/// Добавить устройство в базу данных
		/// </summary>
		public int InsertDevice(string deviceName, string inventoryNumber, string serialNumber, string description, int departmentId, int deviceTypeId)
		{
		    var command =
		        new SqlCommand(
		            @"INSERT INTO Devices([ID Department], [Device Name], [ID Device Type], [SerialNumber], [InventoryNumber], Description, Owner) VALUES (@DepartmentID, @DeviceName, @DeviceTypeID, @SerialNumber, @InventoryNumber, @Description, SUSER_SNAME());
					SET @INSERTED_ID=SCOPE_IDENTITY();") {Connection = _connection};
		    var insertedId = new SqlParameter
		    {
		        ParameterName = "INSERTED_ID",
		        Direction = ParameterDirection.Output,
		        Size = 4
		    };
		    command.Parameters.Add(insertedId);
			command.Parameters.Add(new SqlParameter("DepartmentID", departmentId));
			command.Parameters.Add(new SqlParameter("DeviceName", deviceName));
			command.Parameters.Add(new SqlParameter("DeviceTypeID", deviceTypeId));
			command.Parameters.Add(new SqlParameter("SerialNumber", serialNumber));
			command.Parameters.Add(new SqlParameter("InventoryNumber", inventoryNumber));
			command.Parameters.Add(new SqlParameter("Description", description));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на добавление устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось добавить устройство. "+e.Message, @"Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return -1;
			}
			return int.Parse(insertedId.Value.ToString());
		}

		/// <summary>
		/// Удалить департамент из базы данных
		/// </summary>
		public bool DeleteDepartment(int departmentId)
		{
		    var command = new SqlCommand(@"DELETE FROM Departments WHERE [ID Department] = @DepartmentID")
		    {
		        Connection = _connection
		    };
		    command.Parameters.Add(new SqlParameter("DepartmentID", departmentId));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на удаление департамента", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
				if (e.Number == 547)
					MessageBox.Show(@"Нельзя удалить департамент, если в нем хранится информация об устройствах. Сначала удалите устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось удалить департамент. Подробнее: "+e.Message+@". Код ошибки: "+e.Number, @"Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Удалить устройство из базы данных
		/// </summary>
		public bool DeleteDevice(int deviceId)
		{
		    var command = new SqlCommand(@"DELETE FROM Devices WHERE [ID Device] = @DeviceID") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на удаление устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось удалить устройство. "+e.Message, @"Ошибка", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Добавляет узел в базу
		/// </summary>
		/// <returns>Возвращает идентификатор узла</returns>
		public int InsertDeviceNode(int assocMetaNodeId, int deviceId, string deviceNode)
		{
		    var command = new SqlCommand(@"INSERT INTO Nodes([ID AssocMetaNode], [ID Device], [Value]) 
					VALUES (@AssocMetaNodeID, @DeviceID, @DeviceNode);
					SET @INSERTED_ID=SCOPE_IDENTITY();") {Connection = _connection};
		    var insertedId = new SqlParameter
		    {
		        ParameterName = "INSERTED_ID",
		        Direction = ParameterDirection.Output,
		        Size = 8
		    };
		    command.Parameters.Add(insertedId);
			command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
			command.Parameters.Add(new SqlParameter("AssocMetaNodeID", assocMetaNodeId));
			command.Parameters.Add(new SqlParameter("DeviceNode", deviceNode));
			try
			{
				command.ExecuteNonQuery();
			    command = new SqlCommand(@"select [ID Node] from nodemeta
										where [ID Parent Node] = @AssocMetaNodeID AND [Parameter Name] = 'Модель'") {Connection = _connection};
			    command.Parameters.Add(new SqlParameter("AssocMetaNodeID", assocMetaNodeId));
				var childAssocMetaNodeId = command.ExecuteScalar();
				if (childAssocMetaNodeId != null)
				{
				    command = new SqlCommand(@"INSERT INTO Nodes([ID AssocMetaNode], [ID Parent Node], [ID Device], [Value]) 
					VALUES (@AssocMetaNodeID, @ParentNodeID, @DeviceID, @DeviceNode)") {Connection = _connection};
				    command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
					command.Parameters.Add(new SqlParameter("AssocMetaNodeID", (int)childAssocMetaNodeId));
					command.Parameters.Add(new SqlParameter("DeviceNode", deviceNode));
					command.Parameters.Add(new SqlParameter("ParentNodeID", insertedId.Value.ToString()));
					command.ExecuteNonQuery();
				}
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на добавление узла устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось добавить узел устройтсва. "+e.Message, @"Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return -1;
			}
			return int.Parse(insertedId.Value.ToString());
		}


		/// <summary>
		/// Удалить узел устройства
		/// </summary>
		public bool DeleteDeviceNode(int nodeId)
		{
		    var command = new SqlCommand(@"DELETE FROM Nodes WHERE [ID Node] = @nodeId") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("nodeId", nodeId));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на удаление узла устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось удалить узел устройства. "+e.Message, @"Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Изменить имя устройства
		/// </summary>
		public bool UpdateDeviceNode(int nodeId, string deviceName)
		{
		    var command = new SqlCommand(@"UPDATE Nodes SET Value = @DeviceName WHERE [ID Node] = @nodeId")
		    {
		        Connection = _connection
		    };
		    command.Parameters.Add(new SqlParameter("nodeId", nodeId));
			command.Parameters.Add(new SqlParameter("DeviceName", deviceName));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение узла устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить имя устройства. "+e.Message, @"Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Получить список возможных значений Combobox для заданного поля
		/// </summary>
		public DataView GetValuesByMetaNodeId(int metaNodeId)
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
		    var command = new SqlCommand(@"SELECT * FROM ComboboxValues WHERE [ID Node] = @MetaNodeID")
		    {
		        Connection = _connection
		    };
		    command.Parameters.Add(new SqlParameter("MetaNodeID", metaNodeId));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить справочные значения параметра. "+e.Message, @"Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Добавление новой характеристики устройства
		/// </summary>
		public bool InsertDeviceNodeValue(int assocMetaNodeId, int parentNodeId, int deviceId, string value)
		{
		    var command = new SqlCommand(@"INSERT INTO Nodes ([ID AssocMetaNode], [ID Parent Node], [ID Device], [Value])
												VALUES (@AssocMetaNodeID, @ParentNodeID, @DeviceID, @Value)") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("AssocMetaNodeID", assocMetaNodeId));
			command.Parameters.Add(new SqlParameter("ParentNodeID", parentNodeId));
			command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
			command.Parameters.Add(new SqlParameter("Value", value));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение характеристик устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить характеристику устройства. "+e.Message, @"Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Изменение характеристики устройства
		/// </summary>
		public bool UpdateDeviceNodeValue(int nodeId, string value)
		{
		    var command = new SqlCommand(@"UPDATE Nodes SET Value = @Value WHERE [ID Node] = @nodeId")
		    {
		        Connection = _connection
		    };
		    command.Parameters.Add(new SqlParameter("Value", value));
			command.Parameters.Add(new SqlParameter("nodeId", nodeId));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение характеристик устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить характеристику устройства. "+e.Message, @"Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Вернуть общую информацию об устройстве: серийный и инвентарный номера, сетевое имя, комментарий
		/// </summary>
		public DataView GetDeviceGeneralInfo(int deviceId)
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
		    var command = new SqlCommand(@"select *
							from Devices INNER JOIN [Device Types] ON (Devices.[ID Device Type] = [Device Types].[ID Device Type])
							where [ID Device] =@DeviceID") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию об устройстве. "+e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

        /// <summary>
        /// Вернуть свойства из мониторинга устройства
        /// </summary>
        public DataView GetDeviceMonitoringPropertiesInfo(int deviceId)
        {
            if (_connection.State != ConnectionState.Open)
                return new DataView();
            var command = new SqlCommand(@"SELECT 
                      mp.[Property Name],
                      mp.[Property Value],
                      CASE WHEN mpm.[Display Name] IS NOT NULL THEN mpm.[Display Name] 
                      ELSE mp.[Property Name] END AS [Display Name],
                      mpm.Units,
                      mp.[Update Date]
                    FROM MonitoringProperties mp
                      LEFT JOIN MonitoringPropertiesMeta mpm ON mp.[Property Name] = mpm.[Property Name]
                    WHERE (mpm.Invisible IS NULL OR mpm.Invisible = 0) AND [ID Device] =@DeviceID") { Connection = _connection };
            command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
            var ds = new DataSet();
            try
            {
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить информацию об устройстве. " + e.Message, @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataView();
            }
            return ds.Tables[0].DefaultView;
        }

        public DataView GetDeviceAllMonitoringPropertiesInfo()
        {
            if (_connection.State != ConnectionState.Open)
                return new DataView();
            var command = new SqlCommand(@"SELECT 
                      mp.[ID Device],
                      mp.[Property Name],
                      mp.[Property Value]
                    FROM MonitoringProperties mp
                      LEFT JOIN MonitoringPropertiesMeta mpm ON mp.[Property Name] = mpm.[Property Name]
                    WHERE (mpm.Invisible IS NULL OR mpm.Invisible = 0)") { Connection = _connection };
            var ds = new DataSet();
            try
            {
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить информацию из базы данных. " + e.Message, @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataView();
            }
            return ds.Tables[0].DefaultView;
        }

        /// <summary>
        /// Возвращает все отображаемые свойства из мониторинга
        /// </summary>
        public IEnumerable<MonitoringProperty> GetMonitoringExistsProperties()
        {
            if (_connection.State != ConnectionState.Open)
                return new List<MonitoringProperty>();
            var command =
                new SqlCommand(@"SELECT 
                      DISTINCT mp.[Property Name],
                      CASE WHEN mpm.[Display Name] IS NOT NULL THEN mpm.[Display Name] 
                      ELSE mp.[Property Name] END AS [Display Name]
                    FROM MonitoringProperties mp
                      LEFT JOIN MonitoringPropertiesMeta mpm ON mp.[Property Name] = mpm.[Property Name]
                    WHERE (mpm.Invisible IS NULL OR mpm.Invisible = 0) AND mp.[Property Name] IS NOT NULL")
                {
                    Connection = _connection
                };
            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить список warning-условий мониторинга. " + e.Message, @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<MonitoringProperty>();
            }
            var monitoringProperties = new List<MonitoringProperty>();
            while (reader.Read())
            {
                var propertyName = reader.GetString(0);
                var displayName = reader.GetString(1);
                monitoringProperties.Add(new MonitoringProperty
                {
                    Name = propertyName,
                    DisplayName = displayName
                });
            }
            reader.Close();
            return monitoringProperties;
        }


        /// <summary>
        /// Получить warning-условия свойств мониторинга
        /// </summary>
        public IEnumerable<MonitoringWarningCondition> GetMonitoringWarningConditions()
        {
            if (_connection.State != ConnectionState.Open)
                return new List<MonitoringWarningCondition>();
            var command =
                new SqlCommand(@"SELECT mwc.[Property Name], mwc.[ID Condition Type], mwc.[Condition Bound]
                        FROM MonitoringWarningConditions mwc")
                {
                    Connection = _connection
                };
            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить список warning-условий мониторинга. " + e.Message, @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<MonitoringWarningCondition>();
            }
            var monitoringWarningCondition = new List<MonitoringWarningCondition>();
            while (reader.Read())
            {
                var propertyName = reader.GetString(0);
                var conditionType = (ConditionType)(reader.GetInt32(1) - 1);
                var conditionBound = reader.GetString(2);
                monitoringWarningCondition.Add(new MonitoringWarningCondition
                {
                    PropertyName = propertyName,
                    ConditionType = conditionType,
                    ConditionBound = conditionBound
                });
            }
            reader.Close();
            return monitoringWarningCondition;
        }

		/// <summary>
		/// Вернуть наименование департамента по идентификатору
		/// </summary>
		public string GetDepartmentInfo(int departmentId)
		{
			if (_connection.State != ConnectionState.Open)
				return "";
		    var command = new SqlCommand(@"SELECT * FROM Departments WHERE [ID Department]=@DepartmentID")
		    {
		        Connection = _connection
		    };
		    command.Parameters.Add(new SqlParameter("DepartmentID", departmentId));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию о департаменте. "+e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return "";
			}
			return ds.Tables[0].DefaultView[0]["Department"].ToString();
		}

		/// <summary>
		/// Обновить информацию о департаменте
		/// </summary>
		public bool UpdateDepartment(int departmentId, string department)
		{
		    var command =
		        new SqlCommand(@"UPDATE Departments SET Department = @Department WHERE [ID Department] = @DepartmentID")
		        {
		            Connection = _connection
		        };
		    command.Parameters.Add(new SqlParameter("Department", department));
			command.Parameters.Add(new SqlParameter("DepartmentID", departmentId));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение наименования департамента", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить наименование департамента. "+e.Message, @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Обновить информацию об устройтсве
		/// </summary>
		public bool UpdateDevice(int deviceId, string deviceName, string inventoryNumber, string serialNumber, string description)
		{
		    var command = new SqlCommand(@"UPDATE Devices SET [Device Name] = @DeviceName, InventoryNumber = @InventoryNumber,
								SerialNumber = @SerialNumber, Description = @Description WHERE [ID Device] = @DeviceID")
		    {
		        Connection = _connection
		    };
		    command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
			command.Parameters.Add(new SqlParameter("DeviceName", deviceName));
			command.Parameters.Add(new SqlParameter("InventoryNumber", inventoryNumber));
			command.Parameters.Add(new SqlParameter("SerialNumber", serialNumber));
			command.Parameters.Add(new SqlParameter("Description", description));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение наименования устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить наименование устройства. "+e.Message, @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Получить информацию из архива устройств
		/// </summary>
		public DataView GetArchiveDeviceInfo()
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
		    var command = new SqlCommand(@"SELECT * FROM ArchiveDevicesInfo") {Connection = _connection};
		    var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию из архива устройств. "+e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Получить информацию из архива узлов
		/// </summary>
		/// <returns></returns>
		public DataView GetArchiveNodesInfo()
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
		    var command = new SqlCommand(@"SELECT * FROM ArchiveNodesInfo") {Connection = _connection};
		    var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию из архива устройств. "+e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Получить информацию об удаленных устройствах
		/// </summary>
		/// <returns></returns>
		public DataView GetArchiveDeletedDevicesInfoInfo()
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
		    var command = new SqlCommand(@"SELECT * FROM ArchiveDeletedDevicesInfo") {Connection = _connection};
		    var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию из архива устройств. "+e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Получить информацию об устройстве из архива
		/// </summary>
		public List<Node> GetDeviceInfoFromArchive(int deviceId)
		{
			if (_connection.State != ConnectionState.Open)
				return new List<Node>();
		    var command = new SqlCommand(@"SELECT ArchiveNodes.[ID Node], NodeMeta.[ID Node], [Value]
					FROM dbo.NodeMeta LEFT JOIN dbo.ArchiveNodes ON (NodeMeta.[ID Node] = ArchiveNodes.[ID AssocMetaNode])
					WHERE ([ID Device] = @DeviceID) AND ([Parameter Type] = 'complex') AND (Operation = 'Удаление')
					ORDER BY NodeMeta.[ID Node]") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию о характеристиках компьютера. "+e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var value = reader.GetString(2);
				var nodeId = reader.GetInt32(0);
				int parentNodeId;
				try
				{
					parentNodeId = reader.GetInt32(1);
				}
				catch
				{
					parentNodeId = 0;
				}
				list.Add(new Node(value, nodeId, parentNodeId));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Получить детальную информацию об узле из архива
		/// </summary>
		internal DataView GetDetailDeviceInfoFromArchive(int deviceNodeId)
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
		    var command =
		        new SqlCommand(@"SELECT b.[ID Node] AS NodeRealID, a.[ID Node], b.[ID Parent Node], a.[Parameter Name], 
						a.[Parameter Type], b.Value
				FROM
				(SELECT *
				FROM dbo.NodeMeta
				WHERE dbo.NodeMeta.[ID Parent Node] = (SELECT [ID AssocMetaNode] 
						FROM ArchiveNodes WHERE [ID Node] = @DeviceNodeID AND Operation = 'Удаление')) a
				LEFT JOIN
				(SELECT *
				FROM ArchiveNodes
				WHERE [ID Parent Node] = @DeviceNodeID AND Operation = 'Удаление') b ON (a.[ID Node] = b.[ID AssocMetaNode])")
		        {
		            Connection = _connection
		        };
		    command.Parameters.Add(new SqlParameter("DeviceNodeID", deviceNodeId));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить детальную информацию об узле устройства. "+e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Получить информацию о мета-данных по архиву
		/// </summary>
		/// <returns>Возвращает список характеристик узла устройства</returns>
		public List<Node> GetDeviceInfoMetaInArchive(int deviceId)
		{
			if (_connection.State != ConnectionState.Open)
				return new List<Node>();
		    var command = new SqlCommand(@"SELECT [ID Node], [ID Parent Node], [Parameter Name]
				FROM NodeMeta
				WHERE [Parameter Type] = 'complex' AND [ID Device Type] = (SELECT [ID Device Type]
				FROM ArchiveDevices
				WHERE [ID Device] = @DeviceID AND Operation = 'Удаление')
				ORDER BY [Order], [ID Parent Node]") {Connection = _connection};
		    command.Parameters.Add(new SqlParameter("DeviceID", deviceId));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить мета-данные структуры. "+e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var parameterName = reader.GetString(2);
				var nodeId = reader.GetInt32(0);
				int parentNodeId;
				try
				{
					parentNodeId = reader.GetInt32(1);
				}
				catch
				{
					parentNodeId = 0;
				}
				list.Add(new Node(parameterName, nodeId, parentNodeId));
			}
			reader.Close();
			return list;
		}

		internal DataView GetInstallationsByComputerId(int computerId)
		{
			if (_connection.State != ConnectionState.Open)
				return new DataView();
		    var command =
		        new SqlCommand(
		            @"SELECT [ID Installation], [ID Computer], Software, Version, InstallationDate, LicType, BuyLicenseDate, ExpireLicenseDate, LicKey, SoftMaker, Supplier, SoftType
                                                    FROM
                                                      dbo.SoftInstallations si
                                                        INNER JOIN dbo.SoftLicenses sl ON sl.[ID License] = si.[ID License]
                                                        INNER JOIN dbo.SoftVersions sv ON sl.[ID Version] = sv.[ID Version]
                                                        INNER JOIN dbo.Software s ON s.[ID Software] = sv.[ID Software]
                                                      INNER JOIN dbo.SoftSuppliers ss ON ss.[ID Supplier] = sl.[ID Supplier]
                                                      INNER JOIN dbo.SoftTypes st ON st.[ID SoftType] = s.[ID SoftType]
                                                      INNER JOIN dbo.SoftMakers sm ON sm.[ID SoftMaker] = s.[ID SoftMaker]
                                                      INNER JOIN dbo.SoftLicTypes slt ON slt.[ID LicType] = sl.[ID LicType]
                                                      LEFT JOIN dbo.SoftLicKeys slk ON si.[ID LicenseKey] = slk.[ID LicenseKey]
                                                    WHERE
                                                      [ID Computer] = @ComputerID AND si.[Deleted] <> 1")
		        {
		            Connection = _connection
		        };
		    command.Parameters.Add(new SqlParameter("ComputerID", computerId));
            var ds = new DataSet();
            try
            {
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить детальную информацию об установках ПО. " + e.Message, @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataView();
            }
            return ds.Tables[0].DefaultView;
		}

        internal string GetSerialNumberBy(int id, bool isPeripheralDevice)
        {
            if (_connection.State != ConnectionState.Open)
                return "";
            var command = isPeripheralDevice ? 
                new SqlCommand(@"SELECT Value FROM Nodes WHERE [ID Parent Node] =  @ID AND [ID AssocMetaNode] = 44") : 
                new SqlCommand(@"SELECT SerialNumber FROM Devices WHERE [ID Device] = @ID");
            command.Connection = _connection;
            command.Parameters.Add(new SqlParameter("ID", id));
            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить информацию о компьютере. " + e.Message, @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            string serialNumber;
            if (reader.Read())
            {
                serialNumber = reader[0].ToString();
                reader.Close();
            }
            else
            {
                reader.Close();
                return "";
            }
            return serialNumber;
        }

        internal string GetInventoryNumberBy(int id, bool isPeripheralDevice)
        {
            if (_connection.State != ConnectionState.Open)
                return "";
            var command = isPeripheralDevice ? 
                new SqlCommand(@"SELECT Value FROM Nodes WHERE [ID Parent Node] =  @ID AND [ID AssocMetaNode] = 45") : 
                new SqlCommand(@"SELECT InventoryNumber FROM Devices WHERE [ID Device] = @ID");
            command.Connection = _connection;
            command.Parameters.Add(new SqlParameter("ID", id));
            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить информацию о компьютере. " + e.Message, @"Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            string inventoryNumber;
            if (reader.Read())
            {
                inventoryNumber = reader[0].ToString();
                reader.Close();
            }
            else
            {
                reader.Close();
                return "";
            }
            return inventoryNumber;
        }

		internal DataView GetRequests(string serialNumber, string inventoryNumber)
		{
			var connectionAlexApplic = new SqlConnection(Properties.Settings.Default.AlexApplicConnectionString);
			try
			{
				connectionAlexApplic.Open();
			}
			catch
			{
				MessageBox.Show(@"Ошибка соединения с сервером базы данных AlexApplic. 
								Проверьте правильность строки соединения в файле конфигурации и доступность сервера", @"Ошибка",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		    var commandAlexApplic = new SqlCommand(@"getDevHistory")
		    {
		        CommandType = CommandType.StoredProcedure,
		        Connection = connectionAlexApplic
		    };
		    commandAlexApplic.Parameters.Add(new SqlParameter("SerialNum", serialNumber));
            commandAlexApplic.Parameters.Add(new SqlParameter("InventoryNum", inventoryNumber));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(commandAlexApplic);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить детальную информацию о заявках. " + e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Получить дополнительную информацию, такую как инвентарный и серийный номер, об устройствах в списке
		/// </summary>
		/// <param name="devices"></param>
		public void GetExInfoByDeviceIdList(List<Device> devices)
		{
			if (_connection.State != ConnectionState.Open)
				return;
			var deviceIdList = "";
			for (var i = 0; i < devices.Count; i++)
			{
				deviceIdList += devices[i].DeviceId;
				if (i < devices.Count - 1)
					deviceIdList += ",";
			}
		    var command =
		        new SqlCommand(@"SELECT [ID Device],SerialNumber, InventoryNumber FROM Devices WHERE [ID Device] IN (" +
		                       deviceIdList + ")") {Connection = _connection};

		    var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить детальную информацию об узле устройства. " + e.Message, @"Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				var deviceId = Convert.ToInt32(row["ID Device"]);
				var serialNumber = row["SerialNumber"].ToString();
				var invenotryNumber = row["InventoryNumber"].ToString();
				foreach (var dev in devices)
				{
					if (dev.DeviceId == deviceId)
					{
						dev.SerialNumber = serialNumber;
						dev.InventoryNumber = invenotryNumber;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Перемещение узла из одного департамента в другой
		/// </summary>
		public bool MoveDevice(int deviceId, int newDepartmentId)
		{
			if (_connection.State != ConnectionState.Open)
				return false;
		    var command = new SqlCommand("UPDATE Devices SET [ID Department] = @NewDepartmentID WHERE [ID Device] = @DeviceID")
		    {
		        Connection = _connection
		    };
		    command.Parameters.AddWithValue("@NewDepartmentID", newDepartmentId);
			command.Parameters.AddWithValue("@DeviceID", deviceId);
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на перемещение устройства", @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось переместить устройство. " + e.Message, @"Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

        internal bool MoveDepartment(int departmentId, int newDepartmentId)
        {
            if (_connection.State != ConnectionState.Open)
                return false;
            var command =
                new SqlCommand(
                    "UPDATE Departments SET [ID Parent Department] = @NewDepartmentID WHERE [ID Department] = @DepartmentID")
                {
                    Connection = _connection
                };
            command.Parameters.AddWithValue("@NewDepartmentID", newDepartmentId);
            command.Parameters.AddWithValue("@DepartmentID", departmentId);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (e.Number == 229)
                    MessageBox.Show(@"У вас нет прав на перемещение подразделений", @"Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(@"Не удалось переместить подразделение. " + e.Message, @"Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        internal bool MoveParameter(int nodeId, int newDeviceId)
        {
            if (_connection.State != ConnectionState.Open)
                return false;
            var command = new SqlCommand("UPDATE Nodes SET [ID Device] = @newDeviceId WHERE [ID Node] = @nodeId")
            {
                Connection = _connection
            };
            command.Parameters.AddWithValue("@nodeId", nodeId);
            command.Parameters.AddWithValue("@newDeviceId", newDeviceId);
            var commandCorrection = new SqlCommand(@"UPDATE n2
                                        SET n2.[ID Device] = n1.[ID Device]
                                        FROM Nodes n1
	                                        INNER JOIN Nodes n2 ON n1.[ID Node] = n2.[ID Parent Node]
                                        WHERE n1.[ID Device] <> n2.[ID Device]") {Connection = _connection};
            try
            {
                command.ExecuteNonQuery();
                commandCorrection.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (e.Number == 229)
                    MessageBox.Show(@"У вас нет прав на перемещение характеристики устройства", @"Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(@"Не удалось переместить характеристику устройства. " + e.Message, @"Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
