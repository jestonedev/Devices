CREATE TRIGGER [ArchiveNodesTrigger] ON [Nodes]
AFTER UPDATE
AS
INSERT INTO [ArchiveNodes] ([ID Node], [ID AssocMetaNode], 
	[ID Parent Node], [ID Device], [Value], [Operation]) 
SELECT deleted.[ID Node], deleted.[ID AssocMetaNode],
	deleted.[ID Parent Node], deleted.[ID Device],
	deleted.[Value], 'Изменение'
FROM deleted

CREATE TRIGGER [ArchiveNodesTriggerDelete] ON [Nodes]
AFTER DELETE
AS
INSERT INTO [ArchiveNodes] ([ID Node], [ID AssocMetaNode], 
	[ID Parent Node], [ID Device], [Value], [Operation]) 
SELECT deleted.[ID Node], deleted.[ID AssocMetaNode],
	deleted.[ID Parent Node], deleted.[ID Device],
	deleted.[Value], 'Удаление'
FROM deleted

CREATE TRIGGER [ArchiveDevicesTriggerUpdate] ON [Devices]
AFTER UPDATE
AS
INSERT INTO [ArchiveDevices] ([ID Device], [ID Department], 
	[Device Name], [ID Device Type], SerialNumber, InventoryNumber, 
	[Description], Owner, Operation) 
SELECT deleted.[ID Device], deleted.[ID Department],
	deleted.[Device Name], deleted.[ID Device Type],
	deleted.SerialNumber, deleted.InventoryNumber, deleted.[Description], 
	deleted.Owner, 'Изменение'
FROM deleted

CREATE TRIGGER [ArchiveDevicesTriggerDelete] ON [Devices]
AFTER DELETE
AS
INSERT INTO [ArchiveDevices] ([ID Device], [ID Department], 
	[Device Name], [ID Device Type], SerialNumber, InventoryNumber, 
	[Description], Owner, Operation) 
SELECT deleted.[ID Device], deleted.[ID Department],
	deleted.[Device Name], deleted.[ID Device Type],
	deleted.SerialNumber, deleted.InventoryNumber, deleted.[Description], 
	deleted.Owner, 'Удаление'
FROM deleted