﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
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
		public int NodeID { get; set; }
		public int ParentNodeID { get; set; }

		public Node(string NodeName, int NodeID, int ParentNodeID)
		{
			this.NodeID = NodeID;
			this.NodeName = NodeName;
			this.ParentNodeID = ParentNodeID;
		}

		#region IComparable<Node> Members

		public int CompareTo(Node other)
		{
			return NodeName.CompareTo(other.NodeName);
		}

		#endregion
	}

	internal class DevicesDatabase: IDisposable
	{
		private SqlConnection connection;

		public DevicesDatabase()
		{
			connection = new SqlConnection(Properties.Settings.Default.DevicesConnectionString);
			try
			{
				connection.Open();
			}
			catch
			{
				MessageBox.Show(@"Ошибка соединения с сервером базы данных. Проверьте правильность строки соединения в файле конфигурации и доступность сервера", "Ошибка",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}		

		public void  Dispose()
		{
			connection.Close();
		}
        
        /// <summary>
        /// Возвращает список типов периферийного оборудования
        /// </summary>
        public List<PeripheryType> GetPeripheryType()
        {
            if (connection.State != ConnectionState.Open)
                return new List<PeripheryType>();
            var command = new SqlCommand(@"SELECT cv.[ID Value],cv.Value FROM dbo.ComboboxValues cv WHERE cv.[ID Node]=25");
            command.Connection = connection;
            SqlDataReader reader = null;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить список периферийного оборудования. " + e.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new List<PeripheryType>();
            }
            var PeripheryType = new List<PeripheryType>();            
            while(reader.Read())
            {
                var Id = reader.GetInt32(0);
                var Value = reader.GetString(1);
                PeripheryType.Add(new PeripheryType { Id = Id, Value = Value });
            }
            reader.Close();
            return PeripheryType;
        }
		/// <summary>
		/// Возвращает список всех департаментов
		/// </summary>
		public List<Node> GetDepartments(SearchParametersGroup spg)
		{
			if (connection.State != ConnectionState.Open)
				return new List<Node>();
			var where = "";
			if (spg.departmentIDs.Count > 0)
			{
				where = "[ID Department] IN (";
				foreach (var departmentID in spg.departmentIDs)
				{
					where += departmentID + ",";
				}
				where = where.Trim(',');
				where += ")";
			}

			if (where.Trim().Length > 0)
				where = "WHERE " + where;
		    var command = new SqlCommand(@"SELECT [ID Department], [ID Parent Department], Department
				FROM dbo.Departments " + where + " ORDER BY [ID Parent Department]")
		    {
		        Connection = connection
		    };
		    SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить список департаментов. "+e.Message, "Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var Department = reader.GetString(2);
				var DepartmentID = reader.GetInt32(0);
				int ParentID;
				try {
					ParentID = reader.GetInt32(1);
				}
				catch {
					ParentID = 0;
				}
				list.Add(new Node(Department, DepartmentID, ParentID));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Возвращает список всего сетевого оборудования
		/// </summary>
		public List<Node> GetDevices(SearchParametersGroup spg)
		{
			if (connection.State != ConnectionState.Open)
				return new List<Node>();
			var where = "";
			if (spg.deviceName.Trim().Length > 0)
			{
                where += "([Device Name] LIKE '%" + spg.deviceName.Trim() + "%')";
			}
            if (spg.serialNumber.Trim().Length > 0)
            {
                if (where.Trim().Length > 0)
                    where += " AND ";
                where += "([SerialNumber] LIKE '%" + spg.serialNumber.Trim() + "%')";
            }
            if (spg.inventoryNumber.Trim().Length > 0)
            {
                if (where.Trim().Length > 0)
                    where += " AND ";
                where += "([InventoryNumber] LIKE '%" + spg.inventoryNumber.Trim() + "%')";
            }
            if (spg.deviceTypeID > 0)
		    {
                if (where.Trim().Length > 0)
                    where += " AND ";
                where += "([ID Device Type] = " + spg.deviceTypeID + ")";
		    }
		    if (spg.departmentIDs.Count > 0)
			{
				if (where.Trim().Length > 0)
					where += " AND ";
				where += "[ID Department] IN (";
				foreach (var departmentID in spg.departmentIDs)
				{
					where += departmentID + ",";
				}
				where = where.Trim(',');
				where += ")";
			}
			if (spg.parameters.Count > 0)
			{
				if (where.Trim().Length > 0)
					where += " AND ";
				where += @"[ID Device] IN
						(select [ID Device]
						from dbo.Nodes
						where ";
				var i = 0;
				foreach (var sp in spg.parameters)
				{
                    where += "([ID AssocMetaNode] = " + sp.ParameterId + ") AND ";
				    switch (sp.ParameterType)
				    {
                        case "int":
                            where += "(CAST([Value] AS INT) " + sp.Operation + " " + "CAST(" + sp.ParameterValue + " AS INT))";
				            break;
                        case "float":
                            where += "(CAST(REPLACE([Value],',','.') AS FLOAT) " + sp.Operation + " " + "(CAST(REPLACE("+sp.ParameterValue+",',','.') AS FLOAT)))";
				            break;
                        default:
                            where += "([Value] " + sp.Operation + " '" + sp.ParameterValue + "')";
				            break;
				    }
					i++;
					if (i != spg.parameters.Count)
						where += " OR ";
				}
				where += " group by [ID Device] having COUNT(*) >= " + spg.parameters.Count + ")";
			}
			if (where.Trim().Length > 0)
				where = "WHERE " + where;
		    var command = new SqlCommand(@"SELECT [ID Device], [ID Department], [Device Name]
												FROM Devices " + where) {Connection = connection};
		    SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить список оборудования. "+e.Message, "Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var Device = reader.GetString(2);
				var DeviceID = reader.GetInt32(0);
				int DepartmentID;
				try {
					DepartmentID = reader.GetInt32(1);
				}
				catch {
					DepartmentID = 0;
				}
				list.Add(new Node(Device, DeviceID, DepartmentID));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Возвращает список всех характеристик устройств по типу устройства
		/// </summary>
		/// <param name="DeviceID"></param>
		/// <returns></returns>
		public List<Node> GetDeviceInfoMetaByType(int DeviceTypeID)
		{
			if (connection.State != ConnectionState.Open)
				return new List<Node>();
			var command = new SqlCommand(@"SELECT [ID Node], [ID Parent Node], [Parameter Name]
				FROM NodeMeta
				WHERE [Parameter Type] = 'complex' AND [ID Device Type] = @DeviceTypeID
				ORDER BY [Order], [ID Parent Node]");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceTypeID", DeviceTypeID));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить мета-данные структуры. " + e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var ParameterName = reader.GetString(2);
				var NodeID = reader.GetInt32(0);
				int ParentNodeID;
				try
				{
					ParentNodeID = reader.GetInt32(1);
				}
				catch
				{
					ParentNodeID = 0;
				}
				list.Add(new Node(ParameterName, NodeID, ParentNodeID));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Возвращает список всех характеристик устройства
		/// </summary>
		public List<Node> GetDeviceInfoMeta(int DeviceID)
		{
			if (connection.State != ConnectionState.Open)
				return new List<Node>();
			var command = new SqlCommand(@"SELECT [ID Node], [ID Parent Node], [Parameter Name]
				FROM NodeMeta
				WHERE [Parameter Type] = 'complex' AND [ID Device Type] = (SELECT [ID Device Type]
				FROM Devices
				WHERE [ID Device] = @DeviceID)
				ORDER BY [Order], [ID Parent Node]");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceID",DeviceID));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить мета-данные структуры. "+e.Message, "Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var ParameterName = reader.GetString(2);
				var NodeID = reader.GetInt32(0);
				int ParentNodeID;
				try
				{
					ParentNodeID = reader.GetInt32(1);
				}
				catch
				{
					ParentNodeID = 0;
				}
				list.Add(new Node(ParameterName, NodeID, ParentNodeID));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Получить общую информацию об устройстве
		/// </summary>
		/// <param name="DeviceID">Идентификатор устройства</param>
		/// <returns></returns>
		public List<Node> GetDeviceInfo(int DeviceID)
		{
			if (connection.State != ConnectionState.Open)
				return new List<Node>();
			var command = new SqlCommand(@"SELECT Nodes.[ID Node], NodeMeta.[ID Node], [Value]
					FROM dbo.NodeMeta LEFT JOIN dbo.Nodes ON (NodeMeta.[ID Node] = Nodes.[ID AssocMetaNode])
					WHERE ([ID Device] = @DeviceID) AND ([Parameter Type] = 'complex')
					ORDER BY NodeMeta.[ID Node]");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию о характеристиках компьютера. "+e.Message, "Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var Value = reader.GetString(2);
				var NodeID = reader.GetInt32(0);
				int ParentNodeID;
				try
				{
					ParentNodeID = reader.GetInt32(1);
				}
				catch
				{
					ParentNodeID = 0;
				}
				list.Add(new Node(Value, NodeID, ParentNodeID));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Получить параметры устройства
		/// </summary>
		/// <returns></returns>
		public List<DeviceParametersComboboxItem> GetDeviceParameters(int ParentNodeID)
		{
			if (connection.State != ConnectionState.Open)
				return new List<DeviceParametersComboboxItem>();
			var command = new SqlCommand(@"Select [ID Node], [Parameter Name], [Parameter Type]
				from dbo.NodeMeta
				where [Parameter Type] <> 'complex' AND [ID Parent Node] = @ParentNodeID
				order by [Order]");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("ParentNodeID", ParentNodeID));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию о характеристиках компьютера. " + e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<DeviceParametersComboboxItem>();
			}
			var list = new List<DeviceParametersComboboxItem>();
			while (reader.Read())
			{
				var ParameterName = reader.GetString(1);
				var ParameterType = reader.GetString(2);
				var NodeID = reader.GetInt32(0);
				list.Add(new DeviceParametersComboboxItem(NodeID, ParameterName, ParameterType));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Получить детальную информацию об отдельном узле устройства
		/// </summary>
		/// <param name="NodeID">Идентификатор узла устройтсва</param>
		/// <returns>Возвращает информацию об узле в табличном виде</returns>
		public DataView GetDetailDeviceInfo(int DeviceNodeID)
		{
			if (connection.State != ConnectionState.Open)
				return new DataView();
			var command = new SqlCommand(@"SELECT b.[ID Node] AS NodeRealID, a.[ID Node], b.[ID Parent Node], a.[Parameter Name], 
						a.[Parameter Type], b.Value
				FROM
				(SELECT *
				FROM dbo.NodeMeta
				WHERE dbo.NodeMeta.[ID Parent Node] = (SELECT [ID AssocMetaNode] 
						FROM Nodes WHERE [ID Node] = @DeviceNodeID)) a
				LEFT JOIN
				(SELECT *
				FROM Nodes
				WHERE [ID Parent Node] = @DeviceNodeID) b ON (a.[ID Node] = b.[ID AssocMetaNode])");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceNodeID", DeviceNodeID));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch(SqlException e)
			{
				MessageBox.Show(@"Не удалось получить детальную информацию об узле устройства. "+e.Message, "Ошибка", 
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
			if (connection.State != ConnectionState.Open)
				return new DataView();
			var command = new SqlCommand(@"SELECT * FROM [Device Types]");
			command.Connection = connection;
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить список известных типов устройств. "+e.Message, "Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Добавить департамент в базу данных
		/// </summary>
		/// <returns>Возвращает ID добавленной ноды при успешной операции и -1 при неудаче</returns>
		public int InsertDepartment(string Department, int ParentDepartmentID)
		{
			var command = new SqlCommand(@"INSERT INTO Departments([ID Parent Department], [Department]) VALUES (@ParentDepartmentID, @Department);
					SET @INSERTED_ID=SCOPE_IDENTITY();");
			command.Connection = connection;
			var inserted_id = new SqlParameter();
			inserted_id.ParameterName = "INSERTED_ID";
			inserted_id.Direction = ParameterDirection.Output;
			inserted_id.Size = 4;
			command.Parameters.Add(inserted_id);
			command.Parameters.Add(new SqlParameter("ParentDepartmentID", ParentDepartmentID));
			command.Parameters.Add(new SqlParameter("Department", Department));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на добавление департамента", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось добавить департамент."+e.Message, "Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return -1;
			}
			return Int32.Parse(inserted_id.Value.ToString());
		}

		/// <summary>
		/// Добавить устройство в базу данных
		/// </summary>
		public int InsertDevice(string DeviceName, string InventoryNumber, string SerialNumber, string Description, int DepartmentID, int DeviceTypeID)
		{
			var command = new SqlCommand(@"INSERT INTO Devices([ID Department], [Device Name], [ID Device Type], [SerialNumber], [InventoryNumber], Description, Owner) VALUES (@DepartmentID, @DeviceName, @DeviceTypeID, @SerialNumber, @InventoryNumber, @Description, SUSER_SNAME());
					SET @INSERTED_ID=SCOPE_IDENTITY();");
			command.Connection = connection;
			var inserted_id = new SqlParameter();
			inserted_id.ParameterName = "INSERTED_ID";
			inserted_id.Direction = ParameterDirection.Output;
			inserted_id.Size = 4;
			command.Parameters.Add(inserted_id);
			command.Parameters.Add(new SqlParameter("DepartmentID", DepartmentID));
			command.Parameters.Add(new SqlParameter("DeviceName", DeviceName));
			command.Parameters.Add(new SqlParameter("DeviceTypeID", DeviceTypeID));
			command.Parameters.Add(new SqlParameter("SerialNumber", SerialNumber));
			command.Parameters.Add(new SqlParameter("InventoryNumber", InventoryNumber));
			command.Parameters.Add(new SqlParameter("Description", Description));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на добавление устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось добавить устройство. "+e.Message, "Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return -1;
			}
			return Int32.Parse(inserted_id.Value.ToString());
		}

		/// <summary>
		/// Удалить департамент из базы данных
		/// </summary>
		public bool DeleteDepartment(int DepartmentID)
		{
			var command = new SqlCommand(@"DELETE FROM Departments WHERE [ID Department] = @DepartmentID");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DepartmentID", DepartmentID));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на удаление департамента", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
				if (e.Number == 547)
					MessageBox.Show(@"Нельзя удалить департамент, если в нем хранится информация об устройствах. Сначала удалите устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось удалить департамент. Подробнее: "+e.Message+". Код ошибки: "+e.Number.ToString(), "Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Удалить устройство из базы данных
		/// </summary>
		public bool DeleteDevice(int DeviceID)
		{
			var command = new SqlCommand(@"DELETE FROM Devices WHERE [ID Device] = @DeviceID");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на удаление устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось удалить устройство. "+e.Message, "Ошибка", 
						MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Добавляет узел в базу
		/// </summary>
		/// <returns>Возвращает идентификатор узла</returns>
		public int InsertDeviceNode(int AssocMetaNodeID, int DeviceID, string DeviceNode)
		{
			var command = new SqlCommand(@"INSERT INTO Nodes([ID AssocMetaNode], [ID Device], [Value]) 
					VALUES (@AssocMetaNodeID, @DeviceID, @DeviceNode);
					SET @INSERTED_ID=SCOPE_IDENTITY();");
			command.Connection = connection;
			var inserted_id = new SqlParameter();
			inserted_id.ParameterName = "INSERTED_ID";
			inserted_id.Direction = ParameterDirection.Output;
			inserted_id.Size = 8;
			command.Parameters.Add(inserted_id);
			command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
			command.Parameters.Add(new SqlParameter("AssocMetaNodeID", AssocMetaNodeID));
			command.Parameters.Add(new SqlParameter("DeviceNode", DeviceNode));
			try
			{
				command.ExecuteNonQuery();
				command = new SqlCommand(@"select [ID Node] from nodemeta
										where [ID Parent Node] = @AssocMetaNodeID AND [Parameter Name] = 'Модель'");
				command.Connection = connection;
				command.Parameters.Add(new SqlParameter("AssocMetaNodeID", AssocMetaNodeID));
				var ChildAssocMetaNodeID = command.ExecuteScalar();
				if (ChildAssocMetaNodeID != null)
				{
					command = new SqlCommand(@"INSERT INTO Nodes([ID AssocMetaNode], [ID Parent Node], [ID Device], [Value]) 
					VALUES (@AssocMetaNodeID, @ParentNodeID, @DeviceID, @DeviceNode)");
					command.Connection = connection;
					command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
					command.Parameters.Add(new SqlParameter("AssocMetaNodeID", (int)ChildAssocMetaNodeID));
					command.Parameters.Add(new SqlParameter("DeviceNode", DeviceNode));
					command.Parameters.Add(new SqlParameter("ParentNodeID", inserted_id.Value.ToString()));
					command.ExecuteNonQuery();
				}
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на добавление узла устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось добавить узел устройтсва. "+e.Message, "Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return -1;
			}
			return Int32.Parse(inserted_id.Value.ToString());
		}


		/// <summary>
		/// Удалить узел устройства
		/// </summary>
		public bool DeleteDeviceNode(int NodeID)
		{
			var command = new SqlCommand(@"DELETE FROM Nodes WHERE [ID Node] = @nodeId");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("nodeId", NodeID));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на удаление узла устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось удалить узел устройства. "+e.Message, "Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Изменить имя устройства
		/// </summary>
		public bool UpdateDeviceNode(int NodeID, string DeviceName)
		{
			var command = new SqlCommand(@"UPDATE Nodes SET Value = @DeviceName WHERE [ID Node] = @nodeId");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("nodeId", NodeID));
			command.Parameters.Add(new SqlParameter("DeviceName", DeviceName));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение узла устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить имя устройства. "+e.Message, "Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Получить список возможных значений Combobox для заданного поля
		/// </summary>
		public DataView GetValuesByMetaNodeID(int MetaNodeID)
		{
			if (connection.State != ConnectionState.Open)
				return new DataView();
			var command = new SqlCommand(@"SELECT * FROM ComboboxValues WHERE [ID Node] = @MetaNodeID");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("MetaNodeID", MetaNodeID));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить справочные значения параметра. "+e.Message, "Ошибка", 
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Добавление новой характеристики устройства
		/// </summary>
		public bool InsertDeviceNodeValue(int AssocMetaNodeID, int ParentNodeID, int DeviceID, string Value)
		{
			var command = new SqlCommand(@"INSERT INTO Nodes ([ID AssocMetaNode], [ID Parent Node], [ID Device], [Value])
												VALUES (@AssocMetaNodeID, @ParentNodeID, @DeviceID, @Value)");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("AssocMetaNodeID", AssocMetaNodeID));
			command.Parameters.Add(new SqlParameter("ParentNodeID", ParentNodeID));
			command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
			command.Parameters.Add(new SqlParameter("Value", Value));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение характеристик устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить характеристику устройства. "+e.Message, "Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Изменение характеристики устройства
		/// </summary>
		public bool UpdateDeviceNodeValue(int NodeID, string Value)
		{
			var command = new SqlCommand(@"UPDATE Nodes SET Value = @Value WHERE [ID Node] = @nodeId");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("Value", Value));
			command.Parameters.Add(new SqlParameter("nodeId", NodeID));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение характеристик устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить характеристику устройства. "+e.Message, "Ошибка", 
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Вернуть общую информацию об устройстве: серийный и инвентарный номера, сетевое имя, комментарий
		/// </summary>
		public DataView GetDeviceGeneralInfo(int DeviceID)
		{
			if (connection.State != ConnectionState.Open)
				return new DataView();
			var command = new SqlCommand(@"select *
							from Devices INNER JOIN [Device Types] ON (Devices.[ID Device Type] = [Device Types].[ID Device Type])
							where [ID Device] =@DeviceID");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию об устройстве. "+e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Вернуть наименование департамента по идентификатору
		/// </summary>
		public string GetDepartmentInfo(int DepartmentID)
		{
			if (connection.State != ConnectionState.Open)
				return "";
			var command = new SqlCommand(@"SELECT * FROM Departments WHERE [ID Department]=@DepartmentID");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DepartmentID", DepartmentID));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию о департаменте. "+e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return "";
			}
			return ds.Tables[0].DefaultView[0]["Department"].ToString();
		}

		/// <summary>
		/// Обновить информацию о департаменте
		/// </summary>
		public bool UpdateDepartment(int DepartmentID, string Department)
		{
			var command = new SqlCommand(@"UPDATE Departments SET Department = @Department WHERE [ID Department] = @DepartmentID");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("Department", Department));
			command.Parameters.Add(new SqlParameter("DepartmentID", DepartmentID));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение наименования департамента", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить наименование департамента. "+e.Message, "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Обновить информацию об устройтсве
		/// </summary>
		public bool UpdateDevice(int DeviceID, string DeviceName, string InventoryNumber, string SerialNumber, string Description)
		{
			var command = new SqlCommand(@"UPDATE Devices SET [Device Name] = @DeviceName, InventoryNumber = @InventoryNumber,
								SerialNumber = @SerialNumber, Description = @Description WHERE [ID Device] = @DeviceID");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
			command.Parameters.Add(new SqlParameter("DeviceName", DeviceName));
			command.Parameters.Add(new SqlParameter("InventoryNumber", InventoryNumber));
			command.Parameters.Add(new SqlParameter("SerialNumber", SerialNumber));
			command.Parameters.Add(new SqlParameter("Description", Description));
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на изменение наименования устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось изменить наименование устройства. "+e.Message, "Ошибка",
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
			if (connection.State != ConnectionState.Open)
				return new DataView();
			var command = new SqlCommand(@"SELECT * FROM ArchiveDevicesInfo");
			command.Connection = connection;
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию из архива устройств. "+e.Message, "Ошибка",
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
			if (connection.State != ConnectionState.Open)
				return new DataView();
			var command = new SqlCommand(@"SELECT * FROM ArchiveNodesInfo");
			command.Connection = connection;
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию из архива устройств. "+e.Message, "Ошибка",
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
			if (connection.State != ConnectionState.Open)
				return new DataView();
			var command = new SqlCommand(@"SELECT * FROM ArchiveDeletedDevicesInfo");
			command.Connection = connection;
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию из архива устройств. "+e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Получить информацию об устройстве из архива
		/// </summary>
		public List<Node> GetDeviceInfoFromArchive(int DeviceID)
		{
			if (connection.State != ConnectionState.Open)
				return new List<Node>();
			var command = new SqlCommand(@"SELECT ArchiveNodes.[ID Node], NodeMeta.[ID Node], [Value]
					FROM dbo.NodeMeta LEFT JOIN dbo.ArchiveNodes ON (NodeMeta.[ID Node] = ArchiveNodes.[ID AssocMetaNode])
					WHERE ([ID Device] = @DeviceID) AND ([Parameter Type] = 'complex') AND (Operation = 'Удаление')
					ORDER BY NodeMeta.[ID Node]");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить информацию о характеристиках компьютера. "+e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var Value = reader.GetString(2);
				var NodeID = reader.GetInt32(0);
				int ParentNodeID;
				try
				{
					ParentNodeID = reader.GetInt32(1);
				}
				catch
				{
					ParentNodeID = 0;
				}
				list.Add(new Node(Value, NodeID, ParentNodeID));
			}
			reader.Close();
			return list;
		}

		/// <summary>
		/// Получить детальную информацию об узле из архива
		/// </summary>
		internal DataView GetDetailDeviceInfoFromArchive(int DeviceNodeID)
		{
			if (connection.State != ConnectionState.Open)
				return new DataView();
			var command = new SqlCommand(@"SELECT b.[ID Node] AS NodeRealID, a.[ID Node], b.[ID Parent Node], a.[Parameter Name], 
						a.[Parameter Type], b.Value
				FROM
				(SELECT *
				FROM dbo.NodeMeta
				WHERE dbo.NodeMeta.[ID Parent Node] = (SELECT [ID AssocMetaNode] 
						FROM ArchiveNodes WHERE [ID Node] = @DeviceNodeID AND Operation = 'Удаление')) a
				LEFT JOIN
				(SELECT *
				FROM ArchiveNodes
				WHERE [ID Parent Node] = @DeviceNodeID AND Operation = 'Удаление') b ON (a.[ID Node] = b.[ID AssocMetaNode])");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceNodeID", DeviceNodeID));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить детальную информацию об узле устройства. "+e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new DataView();
			}
			return ds.Tables[0].DefaultView;
		}

		/// <summary>
		/// Получить информацию о мета-данных по архиву
		/// </summary>
		/// <returns>Возвращает список характеристик узла устройства</returns>
		public List<Node> GetDeviceInfoMetaInArchive(int DeviceID)
		{
			if (connection.State != ConnectionState.Open)
				return new List<Node>();
			var command = new SqlCommand(@"SELECT [ID Node], [ID Parent Node], [Parameter Name]
				FROM NodeMeta
				WHERE [Parameter Type] = 'complex' AND [ID Device Type] = (SELECT [ID Device Type]
				FROM ArchiveDevices
				WHERE [ID Device] = @DeviceID AND Operation = 'Удаление')
				ORDER BY [Order], [ID Parent Node]");
			command.Connection = connection;
			command.Parameters.Add(new SqlParameter("DeviceID", DeviceID));
			SqlDataReader reader;
			try
			{
				reader = command.ExecuteReader();
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить мета-данные структуры. "+e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new List<Node>();
			}
			var list = new List<Node>();
			while (reader.Read())
			{
				var ParameterName = reader.GetString(2);
				var NodeID = reader.GetInt32(0);
				int ParentNodeID;
				try
				{
					ParentNodeID = reader.GetInt32(1);
				}
				catch
				{
					ParentNodeID = 0;
				}
				list.Add(new Node(ParameterName, NodeID, ParentNodeID));
			}
			reader.Close();
			return list;
		}

		internal DataView GetInstallationsByComputerID(int ComputerID)
		{
			if (connection.State != ConnectionState.Open)
				return new DataView();
            var command = new SqlCommand(@"SELECT [ID Installation], [ID Computer], Software, Version, InstallationDate, LicType, BuyLicenseDate, ExpireLicenseDate, LicKey, SoftMaker, Supplier, SoftType
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
                                                      [ID Computer] = @ComputerID AND si.[Deleted] <> 1");
			command.Connection = connection;
            command.Parameters.Add(new SqlParameter("ComputerID", ComputerID));
            var ds = new DataSet();
            try
            {
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(ds);
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить детальную информацию об установках ПО. " + e.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataView();
            }
            return ds.Tables[0].DefaultView;
		}

        internal string GetSerialNumberBy(int ID, bool IsPeripheralDevice)
        {
            if (connection.State != ConnectionState.Open)
                return "";
            SqlCommand command = null;
            if (IsPeripheralDevice)
                command = new SqlCommand(@"SELECT Value FROM Nodes WHERE [ID Parent Node] =  @ID AND [ID AssocMetaNode] = 44");
            else
                command = new SqlCommand(@"SELECT SerialNumber FROM Devices WHERE [ID Device] = @ID");
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("ID", ID));
            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить информацию о компьютере. " + e.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            var SerialNumber = "";
            if (reader.Read())
            {
                SerialNumber = reader[0].ToString();
                reader.Close();
            }
            else
            {
                reader.Close();
                return "";
            }
            return SerialNumber;
        }

        internal string GetInventoryNumberBy(int ID, bool IsPeripheralDevice)
        {
            if (connection.State != ConnectionState.Open)
                return "";
            SqlCommand command = null;
            if (IsPeripheralDevice)
                command = new SqlCommand(@"SELECT Value FROM Nodes WHERE [ID Parent Node] =  @ID AND [ID AssocMetaNode] = 45");
            else
                command = new SqlCommand(@"SELECT InventoryNumber FROM Devices WHERE [ID Device] = @ID");
            command.Connection = connection;
            command.Parameters.Add(new SqlParameter("ID", ID));
            SqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (SqlException e)
            {
                MessageBox.Show(@"Не удалось получить информацию о компьютере. " + e.Message, "Ошибка",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "";
            }
            var InventoryNumber = "";
            if (reader.Read())
            {
                InventoryNumber = reader[0].ToString();
                reader.Close();
            }
            else
            {
                reader.Close();
                return "";
            }
            return InventoryNumber;
        }

		internal DataView GetRequests(string SerialNumber, string InventoryNumber)
		{
			var connectionAlexApplic = new SqlConnection(Properties.Settings.Default.AlexApplicConnectionString);
			try
			{
				connectionAlexApplic.Open();
			}
			catch
			{
				MessageBox.Show(@"Ошибка соединения с сервером базы данных AlexApplic. 
								Проверьте правильность строки соединения в файле конфигурации и доступность сервера", "Ошибка",
								MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			var commandAlexApplic = new SqlCommand(@"getDevHistory");
			commandAlexApplic.CommandType = CommandType.StoredProcedure;
			commandAlexApplic.Connection = connectionAlexApplic;
            commandAlexApplic.Parameters.Add(new SqlParameter("SerialNum", SerialNumber));
            commandAlexApplic.Parameters.Add(new SqlParameter("InventoryNum", InventoryNumber));
			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(commandAlexApplic);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить детальную информацию о заявках. " + e.Message, "Ошибка",
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
			if (connection.State != ConnectionState.Open)
				return;
			var deviceID_list = "";
			for (var i = 0; i < devices.Count; i++)
			{
				deviceID_list += devices[i].DeviceID;
				if (i < (devices.Count - 1))
					deviceID_list += ",";
			}
			var command = new SqlCommand(@"SELECT [ID Device],SerialNumber, InventoryNumber FROM Devices WHERE [ID Device] IN ("+deviceID_list+")");
			command.Connection = connection;

			var ds = new DataSet();
			try
			{
				var adapter = new SqlDataAdapter(command);
				adapter.Fill(ds);
			}
			catch (SqlException e)
			{
				MessageBox.Show(@"Не удалось получить детальную информацию об узле устройства. " + e.Message, "Ошибка",
				MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			foreach (DataRow row in ds.Tables[0].Rows)
			{
				var DeviceID = Convert.ToInt32(row["ID Device"]);
				var SerialNumber = row["SerialNumber"].ToString();
				var InvenotryNumber = row["InventoryNumber"].ToString();
				foreach (var dev in devices)
				{
					if (dev.DeviceID == DeviceID)
					{
						dev.SerialNumber = SerialNumber;
						dev.InventoryNumber = InvenotryNumber;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Перемещение узла из одного департамента в другой
		/// </summary>
		public bool MoveDevice(int DeviceID, int NewDepartmentID)
		{
			if (connection.State != ConnectionState.Open)
				return false;
			var command = new SqlCommand("UPDATE Devices SET [ID Department] = @NewDepartmentID WHERE [ID Device] = @DeviceID");
			command.Connection = connection;
			command.Parameters.AddWithValue("@NewDepartmentID", NewDepartmentID);
			command.Parameters.AddWithValue("@DeviceID", DeviceID);
			try
			{
				command.ExecuteNonQuery();
			}
			catch (SqlException e)
			{
				if (e.Number == 229)
					MessageBox.Show(@"У вас нет прав на перемещение устройства", "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				else
					MessageBox.Show(@"Не удалось переместить устройство. " + e.Message, "Ошибка",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

        internal bool MoveDepartment(int DepartmentID, int NewDepartmentID)
        {
            if (connection.State != ConnectionState.Open)
                return false;
            var command = new SqlCommand("UPDATE Departments SET [ID Parent Department] = @NewDepartmentID WHERE [ID Department] = @DepartmentID");
            command.Connection = connection;
            command.Parameters.AddWithValue("@NewDepartmentID", NewDepartmentID);
            command.Parameters.AddWithValue("@DepartmentID", DepartmentID);
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (e.Number == 229)
                    MessageBox.Show(@"У вас нет прав на перемещение подразделений", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(@"Не удалось переместить подразделение. " + e.Message, "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        internal bool MoveParameter(int nodeId, int newDeviceId)
        {
            if (connection.State != ConnectionState.Open)
                return false;
            var command = new SqlCommand("UPDATE Nodes SET [ID Device] = @newDeviceId WHERE [ID Node] = @nodeId")
            {
                Connection = connection
            };
            command.Parameters.AddWithValue("@nodeId", nodeId);
            command.Parameters.AddWithValue("@newDeviceId", newDeviceId);
            var commandCorrection = new SqlCommand(@"UPDATE n2
                                        SET n2.[ID Device] = n1.[ID Device]
                                        FROM Nodes n1
	                                        INNER JOIN Nodes n2 ON n1.[ID Node] = n2.[ID Parent Node]
                                        WHERE n1.[ID Device] <> n2.[ID Device]") {Connection = connection};
            try
            {
                command.ExecuteNonQuery();
                commandCorrection.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (e.Number == 229)
                    MessageBox.Show(@"У вас нет прав на перемещение характеристики устройства", "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(@"Не удалось переместить характеристику устройства. " + e.Message, "Ошибка",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
