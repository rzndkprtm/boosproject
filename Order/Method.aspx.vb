Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Order_Method
    Inherits Page

    <WebMethod>
    Public Shared Function GetOrderHeader(headerId As String) As Object
        Dim orderClass As New OrderClass

        Dim row As DataRow = orderClass.GetDataRow("SELECT OrderId, CustomerId, OrderNumber, OrderName FROM OrderHeaders WHERE Id = '" & headerId & "'")

        If row Is Nothing Then
            Return Nothing
        End If

        Dim dataHeader As New Dictionary(Of String, String) From {
            {"OrderId", row("OrderId").ToString()},
            {"CustomerId", row("CustomerId").ToString()},
            {"OrderNumber", row("OrderNumber").ToString()},
            {"OrderName", row("OrderName").ToString()}
        }

        Return dataHeader
    End Function

    <WebMethod()>
    Public Shared Function StringData(type As String, dataId As String) As String
        Dim orderClass As New OrderClass

        Dim resultName As String = String.Empty
        If type = "DesignName" Then resultName = orderClass.GetDesignName(dataId)
        If type = "BlindName" Then resultName = orderClass.GetBlindName(dataId)
        If type = "ProductName" Then resultName = orderClass.GetProductName(dataId)
        If type = "TubeName" Then resultName = orderClass.GetTubeName(dataId)
        If type = "ControlName" Then resultName = orderClass.GetControlName(dataId)
        If type = "ColourName" Then resultName = orderClass.GetColourName(dataId)
        If type = "FabricName" Then resultName = orderClass.GetFabricName(dataId)
        If type = "BottomName" Then resultName = orderClass.GetBottomName(dataId)
        If type = "CompanyDetailName" Then resultName = orderClass.GetCompanyDetailName(dataId)
        If type = "ChainLength" Then resultName = orderClass.GetChainType(dataId)
        If type = "CompanyOrder" Then resultName = orderClass.GetCompanyIdByOrder(dataId)
        If type = "CompanyDetailOrder" Then resultName = orderClass.GetCompanyDetailIdByOrder(dataId)
        If type = "CustomerPriceAccess" Then resultName = orderClass.GetCustomerPriceAccess(dataId)
        If type = "RoleAccess" Then resultName = orderClass.GetUserRoleName(dataId)
        If type = "OrderContext" Then resultName = orderClass.GetOrderContext(dataId)

        Return resultName
    End Function

    <WebMethod()>
    Public Shared Function ListData(data As JSONList) As List(Of Object)
        Dim orderClass As New OrderClass
        Dim result As New List(Of Object)

        Dim type As String = data.type
        Dim customtype As String = data.customtype
        Dim designtype As String = data.designtype
        Dim blindtype As String = data.blindtype
        Dim tubetype As String = data.tubetype
        Dim controltype As String = data.controltype
        Dim fabrictype As String = data.fabrictype
        Dim bottomtype As String = data.bottomtype
        Dim chaincolour As String = data.chaincolour
        Dim companyid As String = data.companyid
        Dim companydetailid As String = data.companydetailid
        Dim action As String = data.action

        If type = "BlindType" Then
            Dim param As New List(Of SqlParameter) From {
                New SqlParameter("@Type", type),
                New SqlParameter("@DesignType", designtype),
                New SqlParameter("@CompanyDetail", companydetailid),
                New SqlParameter("@Action", action)
            }
            Dim dt As DataTable = orderClass.GetDataTableSP("sp_GetBlindTypeForOrderDetail", param)

            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "BlindTypeCS" Then
            Dim param As New List(Of SqlParameter) From {
                New SqlParameter("@Type", type),
                New SqlParameter("@DesignType", designtype),
                New SqlParameter("@CompanyDetail", companydetailid),
                New SqlParameter("@Action", action)
            }
            Dim dt As DataTable = orderClass.GetDataTableSP("sp_GetBlindTypeForOrderDetail", param)

            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "BlindTypeRoller" Then
            Dim param As New List(Of SqlParameter) From {
                New SqlParameter("@Type", type),
                New SqlParameter("@DesignType", designtype),
                New SqlParameter("@CompanyDetail", companydetailid),
                New SqlParameter("@Action", action)
            }
            Dim dt As DataTable = orderClass.GetDataTableSP("sp_GetBlindTypeForOrderDetail", param)

            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "BlindTypeShutter" Then
            Dim param As New List(Of SqlParameter) From {
                New SqlParameter("@Type", type),
                New SqlParameter("@DesignType", designtype),
                New SqlParameter("@CompanyDetail", companydetailid),
                New SqlParameter("@Action", action)
            }
            Dim dt As DataTable = orderClass.GetDataTableSP("sp_GetBlindTypeForOrderDetail", param)

            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "TubeType" Then
            Dim thisQuery As String = "SELECT Products.TubeType AS TextValue, ProductTubes.Name AS TextName FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS companyArray INNER JOIN ProductTubes ON Products.TubeType=ProductTubes.Id WHERE Products.BlindId='" & blindtype & "' AND companyArray.VALUE='" & companydetailid & "' AND Products.Active=1 GROUP BY Products.TubeType, ProductTubes.Name ORDER BY ProductTubes.Name ASC"
            If action = "view" Then
                thisQuery = "SELECT Products.TubeType AS TextValue, ProductTubes.Name AS TextName FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS companyArray INNER JOIN ProductTubes ON Products.TubeType=ProductTubes.Id WHERE Products.BlindId='" & blindtype & "' AND companyArray.VALUE='" & companydetailid & "' GROUP BY Products.TubeType, ProductTubes.Name ORDER BY ProductTubes.Name ASC"
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("TextValue").ToString(), .Text = row("TextName").ToString()})
            Next
        End If

        If type = "ControlType" Then
            Dim thisQuery As String = "SELECT Products.ControlType AS TextValue, ProductControls.Name AS TextName FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS companyArray INNER JOIN ProductControls ON Products.ControlType=ProductControls.Id WHERE Products.BlindId='" & blindtype & "' AND Products.TubeType='" & tubetype & "' AND companyArray.VALUE='" & companydetailid & "' AND Products.Active=1 GROUP BY Products.ControlType, ProductControls.Name ORDER BY ProductControls.Name ASC"
            If action = "view" Then
                thisQuery = "SELECT Products.ControlType AS TextValue, ProductControls.Name AS TextName FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS companyArray INNER JOIN ProductControls ON Products.ControlType=ProductControls.Id WHERE Products.BlindId='" & blindtype & "' AND Products.TubeType='" & tubetype & "' AND companyArray.VALUE='" & companydetailid & "' GROUP BY Products.ControlType, ProductControls.Name ORDER BY ProductControls.Name ASC"
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("TextValue").ToString(), .Text = row("TextName").ToString()})
            Next
        End If

        If type = "ColourType" Then
            Dim thisQuery As String = "SELECT Products.*, ProductColours.Name AS ColourName FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS companyArray INNER JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE Products.BlindId='" & blindtype & "' AND companyArray.VALUE='" & companydetailid & "' AND Products.TubeType='" & tubetype & "' AND Products.ControlType='" & controltype & "' AND Products.Active=1 ORDER BY ProductColours.Name ASC"
            If action = "view" Then
                thisQuery = "SELECT Products.*, ProductColours.Name AS ColourName FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS companyArray INNER JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE Products.BlindId='" & blindtype & "' AND companyArray.VALUE='" & companydetailid & "' AND Products.TubeType='" & tubetype & "' AND Products.ControlType='" & controltype & "' ORDER BY ProductColours.Name ASC"
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("ColourName").ToString()})
            Next
        End If

        If type = "ProductName" Then
            Dim thisQuery As String = "SELECT * FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS companyArray INNER JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE Products.BlindId='" & blindtype & "' AND companyArray.VALUE='" & companydetailid & "' AND Products.TubeType='" & tubetype & "' AND Products.ControlType='" & controltype & "' AND Products.Active=1 ORDER BY Products.Name ASC"
            If action = "view" Then
                thisQuery = "SELECT * FROM Products CROSS APPLY STRING_SPLIT(Products.CompanyDetailId, ',') AS companyArray INNER JOIN ProductColours ON Products.ColourType=ProductColours.Id WHERE Products.BlindId='" & blindtype & "' AND companyArray.VALUE='" & companydetailid & "' AND Products.TubeType='" & tubetype & "' AND Products.ControlType='" & controltype & "' ORDER BY Products.Name ASC"
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "Mounting" Then
            Dim thisQuery As String = "SELECT Name FROM Mountings CROSS APPLY STRING_SPLIT(BlindId, ',') AS blindArray WHERE blindArray.VALUE='" & blindtype & "' AND Active=1 ORDER BY Name ASC"
            If action = "view" Then
                thisQuery = "SELECT Name FROM Mountings CROSS APPLY STRING_SPLIT(BlindId, ',') AS blindArray WHERE blindArray.VALUE='" & blindtype & "' ORDER BY Name ASC"
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Name").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "FabricType" Then
            Dim thisQuery As String = "SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND tubeArray.VALUE='" & tubetype & "' AND companyArray.VALUE='" & companydetailid & "' AND Active=1 ORDER BY Name ASC"
            If action = "view" Then
                thisQuery = "SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeId, ',') AS tubeArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND tubeArray.VALUE='" & tubetype & "' AND companyArray.VALUE='" & companydetailid & "' ORDER BY Name ASC"
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "FabricTypeByDesign" Then
            Dim thisQuery As String = "SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND companyArray.VALUE='" & companydetailid & "' AND Active=1 ORDER BY Name ASC"
            If designtype = "3" Then
                thisQuery = "SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND companyArray.VALUE='" & companydetailid & "' AND Active=1 ORDER BY CASE WHEN [Group]='Group Express' THEN 1 ELSE 2 END, Name ASC"
            End If
            If action = "view" Then
                thisQuery = "SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND companyArray.VALUE='" & companydetailid & "' ORDER BY Name ASC"
                If designtype = "3" Then
                    thisQuery = "SELECT * FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND companyArray.VALUE='" & companydetailid & "' ORDER BY CASE WHEN [Group]='Group Express' THEN 1 ELSE 2 END, Name ASC"
                End If
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "FabricColour" Then
            Dim thisQuery As String = "SELECT * FROM FabricColours WHERE FabricId='" & fabrictype & "' AND Active=1 ORDER BY Colour ASC"
            If action = "view" Then
                thisQuery = "SELECT * FROM FabricColours WHERE FabricId='" & fabrictype & "' ORDER BY Colour ASC"
            End If
            If companydetailid = "4" OrElse companydetailid = "5" Then
                thisQuery = "SELECT * FROM FabricColours WHERE FabricId='" & fabrictype & "' AND Factory='Express' AND Active=1 ORDER BY Colour ASC"
                If action = "view" Then
                    thisQuery = "SELECT * FROM FabricColours WHERE FabricId='" & fabrictype & "' AND Factory='Express' ORDER BY Colour ASC"
                End If
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Colour").ToString()})
            Next
        End If

        If type = "ControlColour" Then
            Dim thisQuery As String = "SELECT * FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND controlArray.VALUE='" & controltype & "' AND companyArray.VALUE='" & companydetailid & "' AND Active=1 ORDER BY Name ASC"
            If action = "view" Then
                thisQuery = "SELECT * FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND controlArray.VALUE='" & controltype & "' AND companyArray.VALUE='" & companydetailid & "' ORDER BY Name ASC"
            End If

            If customtype = "Cassette" Then
                thisQuery = "SELECT * FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND controlArray.VALUE='" & controltype & "' AND companyArray.VALUE='" & companydetailid & "' AND Active=1 ORDER BY Name ASC"
                If action = "view" Then
                    thisQuery = "SELECT * FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND controlArray.VALUE='" & controltype & "' AND companyArray.VALUE='" & companydetailid & "' ORDER BY Name ASC"
                End If

                If controltype = "1" Then
                    thisQuery = "SELECT * FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND controlArray.VALUE='" & controltype & "' AND companyArray.VALUE='" & companydetailid & "' AND ChainType='Continuous' AND Active=1 ORDER BY Name ASC"
                    If action = "view" Then
                        thisQuery = "SELECT * FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlTypeId, ',') AS controlArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND controlArray.VALUE='" & controltype & "' AND companyArray.VALUE='" & companydetailid & "' AND ChainType='Continuous' ORDER BY Name ASC"
                    End If
                End If
            End If
            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "BottomType" Then
            Dim thisQuery As String = "SELECT * FROM Bottoms CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND companyArray.VALUE='" & companydetailid & "' AND Active=1 ORDER BY Name ASC"
            If action = "view" Then
                thisQuery = "SELECT * FROM Bottoms CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS companyArray WHERE designArray.VALUE='" & designtype & "' AND companyArray.VALUE='" & companydetailid & "' ORDER BY Name ASC"
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Name").ToString()})
            Next
        End If

        If type = "BottomColour" Then
            Dim thisQuery As String = "SELECT * FROM BottomColours WHERE BottomId='" & bottomtype & "' AND Active=1 ORDER BY Colour ASC"
            If action = "view" Then
                thisQuery = "SELECT * FROM BottomColours WHERE BottomId='" & bottomtype & "' ORDER BY Colour ASC"
            End If

            Dim dt As DataTable = orderClass.GetDataTable(thisQuery)
            For Each row As DataRow In dt.Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Colour").ToString()})
            Next
        End If

        If type = "ChainStopper" Then
            Dim chainType As String = orderClass.GetItemData("SELECT ChainType FROM Chains WHERE Id='" & chaincolour & "'")
            If chainType = "Continuous" Then
                result.Add(New With {.Value = "With Stopper", .Text = "With Stopper"})
                result.Add(New With {.Value = "No Stopper", .Text = "No Stopper"})
            End If
            If chainType = "Non Continuous" Then
                result.Add(New With {.Value = "With Stopper", .Text = "With Stopper"})
            End If
        End If

        If type = "CurtainTrackType" Then
            If Not String.IsNullOrEmpty(customtype) Then
                result.Add(New With {.Value = "Styletrack", .Text = "Styletrack"})
                result.Add(New With {.Value = "Commercial", .Text = "Commercial"})

                If customtype = "S-Wave" Then
                    result.Clear()
                    result.Add(New With {.Value = "Commercial", .Text = "Commercial"})
                End If
            End If
        End If

        If type = "CurtainTrackColour" Then
            If Not String.IsNullOrEmpty(customtype) Then
                result.Add(New With {.Value = "Black", .Text = "Black"})
                result.Add(New With {.Value = "Birch White", .Text = "Birch White"})
                result.Add(New With {.Value = "Matt Satin", .Text = "Matt Satin"})
                result.Add(New With {.Value = "White", .Text = "White"})
            End If
        End If

        If type = "LayoutCodePG" Then
            If Not String.IsNullOrEmpty(customtype) Then
                result.Add(New With {.Value = "A", .Text = "A"})
                result.Add(New With {.Value = "B", .Text = "B"})
                result.Add(New With {.Value = "C", .Text = "C"})
                result.Add(New With {.Value = "D", .Text = "D"})
                result.Add(New With {.Value = "E", .Text = "E"})
                result.Add(New With {.Value = "F", .Text = "F"})
                result.Add(New With {.Value = "S", .Text = "S"})
                If customtype = "2" Then
                    result.Clear()
                    result.Add(New With {.Value = "A", .Text = "A"})
                    result.Add(New With {.Value = "B", .Text = "B"})
                    result.Add(New With {.Value = "C", .Text = "C"})
                    result.Add(New With {.Value = "D", .Text = "D"})
                    result.Add(New With {.Value = "S", .Text = "S"})
                End If
            End If
        End If

        If type = "LayoutCodeDoor" Then
            Dim tubeName As String = orderClass.GetTubeName(tubetype)
            If tubeName = "Hinged Single" Then
                result.Add(New With {.Value = "L", .Text = "L"})
                result.Add(New With {.Value = "R", .Text = "R"})
            End If
            If tubeName = "Hinged Double" Then
                result.Add(New With {.Value = "L-RA", .Text = "L-RA"})
                result.Add(New With {.Value = "AL-R", .Text = "AL-R"})
            End If
            If tubeName = "Sliding Single" Then
                result.Add(New With {.Value = "AL", .Text = "AL"})
                result.Add(New With {.Value = "RA", .Text = "RA"})
            End If
            If tubeName = "Sliding Double" Then
                result.Add(New With {.Value = "AL", .Text = "AL"})
                result.Add(New With {.Value = "RA", .Text = "RA"})
            End If
            If tubeName = "Sliding Stacker" Then
                result.Add(New With {.Value = "AL", .Text = "AL"})
                result.Add(New With {.Value = "RA", .Text = "RA"})
            End If
        End If

        If type = "MeshDoor" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If blindName = "Safety" Then
                result.Add(New With {.Value = "304 SS Mesh", .Text = "304 SS Mesh"})
            End If
            If blindName = "Standard" Then
                result.Add(New With {.Value = "Fibreglass Mesh", .Text = "Fibreglass Mesh"})
                result.Add(New With {.Value = "Pawproof", .Text = "Pawproof"})
                result.Add(New With {.Value = "SS Mesh", .Text = "SS Mesh"})
            End If
            If blindName = "Security" Then
                result.Add(New With {.Value = "316 SS Mesh", .Text = "316 SS Mesh"})
            End If
            If blindName = "Flyscreen" Then
                result.Add(New With {.Value = "Fibreglass Mesh", .Text = "Fibreglass Mesh"})
                result.Add(New With {.Value = "Pawproof", .Text = "Pawproof"})
                result.Add(New With {.Value = "SS Mesh", .Text = "SS Mesh"})
            End If
        End If

        If type = "MeshWindow" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If blindName = "Safety" Then
                result.Add(New With {.Value = "304 SS Mesh", .Text = "304 SS Mesh"})
            End If
            If blindName = "Standard" Then
                result.Add(New With {.Value = "Fibreglass Mesh", .Text = "Fibreglass Mesh"})
                result.Add(New With {.Value = "Pawproof", .Text = "Pawproof"})
                result.Add(New With {.Value = "SS Mesh", .Text = "SS Mesh"})
            End If
            If blindName = "Security" Then
                result.Add(New With {.Value = "316 SS Mesh", .Text = "316 SS Mesh"})
            End If
            If blindName = "Flyscreen" Then
                result.Add(New With {.Value = "Fibreglass Mesh", .Text = "Fibreglass Mesh"})
                result.Add(New With {.Value = "Pawproof", .Text = "Pawproof"})
                result.Add(New With {.Value = "SS Mesh", .Text = "SS Mesh"})
            End If
        End If

        If type = "FrameColourDoor" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If blindName = "Safety" Then
                result.Add(New With {.Value = "Black (Express)", .Text = "Black (Express)"})
                result.Add(New With {.Value = "Monument (Express)", .Text = "Monument (Express)"})
                result.Add(New With {.Value = "Primrose (Express)", .Text = "Primrose (Express)"})
                result.Add(New With {.Value = "White (Express)", .Text = "White (Express)"})
                result.Add(New With {.Value = "White Birch (Express)", .Text = "White Birch (Express)"})
            End If
            If blindName = "Standard" Then
                result.Add(New With {.Value = "Black (Express)", .Text = "Black (Express)"})
                result.Add(New With {.Value = "Monument (Express)", .Text = "Monument (Express)"})
                result.Add(New With {.Value = "Primrose (Express)", .Text = "Primrose (Express)"})
                result.Add(New With {.Value = "White (Express)", .Text = "White (Express)"})
                result.Add(New With {.Value = "White Birch (Express)", .Text = "White Birch (Express)"})
            End If
            If blindName = "Security" Then
                result.Add(New With {.Value = "Black (Express)", .Text = "Black (Express)"})
                result.Add(New With {.Value = "Monument (Express)", .Text = "Monument (Express)"})
                result.Add(New With {.Value = "Primrose (Express)", .Text = "Primrose (Express)"})
                result.Add(New With {.Value = "White (Express)", .Text = "White (Express)"})
                result.Add(New With {.Value = "White Birch (Express)", .Text = "White Birch (Express)"})
            End If
            If blindName = "Flyscreen" Then
                result.Add(New With {.Value = "Black (Express)", .Text = "Black (Express)"})
                result.Add(New With {.Value = "Monument (Express)", .Text = "Monument (Express)"})
                result.Add(New With {.Value = "Primrose (Express)", .Text = "Primrose (Express)"})
                result.Add(New With {.Value = "White (Express)", .Text = "White (Express)"})
                result.Add(New With {.Value = "White Birch (Express)", .Text = "White Birch (Express)"})
            End If

            If Not String.IsNullOrEmpty(blindName) AndAlso (companydetailid = "2" OrElse companydetailid = "3") Then
                result.Add(New With {.Value = "Apo Grey (Regular)", .Text = "Apo Grey (Regular)"})
                result.Add(New With {.Value = "Beige (Regular)", .Text = "Beige (Regular)"})
                result.Add(New With {.Value = "Birch White (Regular)", .Text = "Birch White (Regular)"})
                result.Add(New With {.Value = "Black (Regular)", .Text = "Black (Regular)"})
                result.Add(New With {.Value = "Brown (Regular)", .Text = "Brown (Regular)"})
                result.Add(New With {.Value = "Charcoal (Regular)", .Text = "Charcoal (Regular)"})
                result.Add(New With {.Value = "Deep Ocean (Regular)", .Text = "Deep Ocean (Regular)"})
                result.Add(New With {.Value = "Dune (Regular)", .Text = "Dune (Regular)"})
                result.Add(New With {.Value = "Hawthorne Green (Regular)", .Text = "Hawthorne Green (Regular)"})
                result.Add(New With {.Value = "Jasper (Regular)", .Text = "Jasper (Regular)"})
                result.Add(New With {.Value = "Monument (Regular)", .Text = "Monument (Regular)"})
                result.Add(New With {.Value = "Notre Dame (Regular)", .Text = "Notre Dame (Regular)"})
                result.Add(New With {.Value = "Pale Eucalypt (Regular)", .Text = "Pale Eucalypt (Regular)"})
                result.Add(New With {.Value = "Paperbark (Regular)", .Text = "Paperbark (Regular)"})
                result.Add(New With {.Value = "Primrose (Regular)", .Text = "Primrose (Regular)"})
                result.Add(New With {.Value = "Silver (Regular)", .Text = "Silver (Regular)"})
                result.Add(New With {.Value = "Surf Mist (Regular)", .Text = "Surf Mist (Regular)"})
                result.Add(New With {.Value = "White (Regular)", .Text = "White (Regular)"})
                result.Add(New With {.Value = "Woodland Grey (Regular)", .Text = "Woodland Grey (Regular)"})
            End If
        End If

        If type = "FrameColourWindow" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If blindName = "Safety" Then
                result.Add(New With {.Value = "Black (Express)", .Text = "Black (Express)"})
                result.Add(New With {.Value = "Monument (Express)", .Text = "Monument (Express)"})
                result.Add(New With {.Value = "Primrose (Express)", .Text = "Primrose (Express)"})
                result.Add(New With {.Value = "White (Express)", .Text = "White (Express)"})
                result.Add(New With {.Value = "White Birch (Express)", .Text = "White Birch (Express)"})
            End If
            If blindName = "Standard" Then
                result.Add(New With {.Value = "Black (Express)", .Text = "Black (Express)"})
                result.Add(New With {.Value = "Monument (Express)", .Text = "Monument (Express)"})
                result.Add(New With {.Value = "Primrose (Express)", .Text = "Primrose (Express)"})
                result.Add(New With {.Value = "White (Express)", .Text = "White (Express)"})
                result.Add(New With {.Value = "White Birch (Express)", .Text = "White Birch (Express)"})
            End If
            If blindName = "Security" Then
                result.Add(New With {.Value = "Black (Express)", .Text = "Black (Express)"})
                result.Add(New With {.Value = "Monument (Express)", .Text = "Monument (Express)"})
                result.Add(New With {.Value = "Primrose (Express)", .Text = "Primrose (Express)"})
                result.Add(New With {.Value = "White (Express)", .Text = "White (Express)"})
                result.Add(New With {.Value = "White Birch (Express)", .Text = "White Birch (Express)"})
            End If
            If blindName = "Flyscreen" Then
                result.Add(New With {.Value = "Black (Express)", .Text = "Black (Express)"})
                result.Add(New With {.Value = "Monument (Express)", .Text = "Monument (Express)"})
                result.Add(New With {.Value = "Primrose (Express)", .Text = "Primrose (Express)"})
                result.Add(New With {.Value = "White (Express)", .Text = "White (Express)"})
                result.Add(New With {.Value = "White Birch (Express)", .Text = "White Birch (Express)"})
            End If

            If Not String.IsNullOrEmpty(blindName) AndAlso (companydetailid = "2" OrElse companydetailid = "3") Then
                result.Add(New With {.Value = "Apo Grey (Regular)", .Text = "Apo Grey (Regular)"})
                result.Add(New With {.Value = "Beige (Regular)", .Text = "Beige (Regular)"})
                result.Add(New With {.Value = "Birch White (Regular)", .Text = "Birch White (Regular)"})
                result.Add(New With {.Value = "Black (Regular)", .Text = "Black (Regular)"})
                result.Add(New With {.Value = "Brown (Regular)", .Text = "Brown (Regular)"})
                result.Add(New With {.Value = "Charcoal (Regular)", .Text = "Charcoal (Regular)"})
                result.Add(New With {.Value = "Deep Ocean (Regular)", .Text = "Deep Ocean (Regular)"})
                result.Add(New With {.Value = "Dune (Regular)", .Text = "Dune (Regular)"})
                result.Add(New With {.Value = "Hawthorne Green (Regular)", .Text = "Hawthorne Green (Regular)"})
                result.Add(New With {.Value = "Jasper (Regular)", .Text = "Jasper (Regular)"})
                result.Add(New With {.Value = "Monument (Regular)", .Text = "Monument (Regular)"})
                result.Add(New With {.Value = "Notre Dame (Regular)", .Text = "Notre Dame (Regular)"})
                result.Add(New With {.Value = "Pale Eucalypt (Regular)", .Text = "Pale Eucalypt (Regular)"})
                result.Add(New With {.Value = "Paperbark (Regular)", .Text = "Paperbark (Regular)"})
                result.Add(New With {.Value = "Primrose (Regular)", .Text = "Primrose (Regular)"})
                result.Add(New With {.Value = "Silver (Regular)", .Text = "Silver (Regular)"})
                result.Add(New With {.Value = "Surf Mist (Regular)", .Text = "Surf Mist (Regular)"})
                result.Add(New With {.Value = "White (Regular)", .Text = "White (Regular)"})
                result.Add(New With {.Value = "Woodland Grey (Regular)", .Text = "Woodland Grey (Regular)"})
            End If
        End If

        If type = "InterlockDoor" Then
            Dim tubeName As String = orderClass.GetTubeName(tubetype)
            If tubeName = "Sliding Single" Then
                result.Add(New With {.Value = "HD3 Offset (5mm)", .Text = "HD3 Offset (5mm)"})
                result.Add(New With {.Value = "HD2 Flat (1.5mm)", .Text = "HD2 Flat (1.5mm)"})
                result.Add(New With {.Value = "HD10 Large Offset", .Text = "HD10 Large Offset"})
                result.Add(New With {.Value = "HD9 F Interlock", .Text = "HD9 F Interlock"})
            End If
            If tubeName = "Sliding Double" Then
                result.Add(New With {.Value = "HD9 F Interlock", .Text = "HD9 F Interlock"})
            End If
            If tubeName = "Sliding Stacker" Then
                result.Add(New With {.Value = "HD3 Offset (5 mm) with HD9 F Interlock", .Text = "HD3 Offset (5 mm) with HD9 F Interlock"})
                result.Add(New With {.Value = "HD2 Flat (1.5 mm) with HD9 F Interlock", .Text = "HD2 Flat (1.5 mm) with HD9 F Interlock"})
                result.Add(New With {.Value = "HD10 Large Offset with HD9 F Interlock", .Text = "HD10 Large Offset with HD9 F Interlock"})
            End If
        End If

        If type = "ValanceRoman" Then
            Dim controlName As String = orderClass.GetControlName(controltype)
            If Not String.IsNullOrEmpty(controlName) Then
                result.Add(New With {.Value = "Facade", .Text = "Facade"})
                result.Add(New With {.Value = "Retrousse", .Text = "Retrousse"})
                If controlName = "Chain" Then
                    result.Clear()
                    result.Add(New With {.Value = "Retrousse", .Text = "Retrousse"})
                End If
            End If
        End If

        If type = "SubType" Then
            Dim blindName As String = orderClass.GetItemData("SELECT Name FROM Blinds WHERE Id='" & blindtype & "'")

            If blindName = "Aluminium 25mm x 0.21mm" Then
                result.Add(New With {.Value = "Single", .Text = "Single Aluminium"})
                result.Add(New With {.Value = "2 on 1 Left-Left", .Text = "2 on 1 Aluminium (Left-Left)"})
                result.Add(New With {.Value = "2 on 1 Right-Right", .Text = "2 on 1 Aluminium (Right-Right)"})
                result.Add(New With {.Value = "2 on 1 Left-Right", .Text = "2 on 1 Aluminium (Left-Right)"})
            End If

            If blindName = "Basswood 50mm" Then
                result.Add(New With {.Value = "Single", .Text = "Single Venetian"})
                result.Add(New With {.Value = "2 on 1 Left-Left", .Text = "2 on 1 Venetian (Left-Left)"})
                result.Add(New With {.Value = "2 on 1 Right-Right", .Text = "2 on 1 Venetian (Right-Right)"})
                result.Add(New With {.Value = "2 on 1 Left-Right", .Text = "2 on 1 Venetian (Left-Right)"})
            End If

            If blindName = "Basswood 63mm" Then
                result.Add(New With {.Value = "Single", .Text = "Single Venetian"})
                result.Add(New With {.Value = "2 on 1 Left-Left", .Text = "2 on 1 Venetian (Left-Left)"})
                result.Add(New With {.Value = "2 on 1 Right-Right", .Text = "2 on 1 Venetian (Right-Right)"})
                result.Add(New With {.Value = "2 on 1 Left-Right", .Text = "2 on 1 Venetian (Left-Right)"})
            End If

            If blindName = "Econo 50mm" Then
                result.Add(New With {.Value = "Single", .Text = "Single Venetian"})
                result.Add(New With {.Value = "2 on 1 Left-Left", .Text = "2 on 1 Venetian (Left-Left)"})
                result.Add(New With {.Value = "2 on 1 Right-Right", .Text = "2 on 1 Venetian (Right-Right)"})
                result.Add(New With {.Value = "2 on 1 Left-Right", .Text = "2 on 1 Venetian (Left-Right)"})
            End If

            If blindName = "Econo 63mm" Then
                result.Add(New With {.Value = "Single", .Text = "Single Venetian"})
                result.Add(New With {.Value = "2 on 1 Left-Left", .Text = "2 on 1 Venetian (Left-Left)"})
                result.Add(New With {.Value = "2 on 1 Right-Right", .Text = "2 on 1 Venetian (Right-Right)"})
                result.Add(New With {.Value = "2 on 1 Left-Right", .Text = "2 on 1 Venetian (Left-Right)"})
            End If

            If blindName = "Ultraslat 50mm" Then
                result.Add(New With {.Value = "Single", .Text = "Single Venetian"})
                result.Add(New With {.Value = "2 on 1 Left-Left", .Text = "2 on 1 Venetian (Left-Left)"})
                result.Add(New With {.Value = "2 on 1 Right-Right", .Text = "2 on 1 Venetian (Right-Right)"})
                result.Add(New With {.Value = "2 on 1 Left-Right", .Text = "2 on 1 Venetian (Left-Right)"})
            End If

            If blindName = "Ultraslat 63mm" Then
                result.Add(New With {.Value = "Single", .Text = "Single Venetian"})
                result.Add(New With {.Value = "2 on 1 Left-Left", .Text = "2 on 1 Venetian (Left-Left)"})
                result.Add(New With {.Value = "2 on 1 Right-Right", .Text = "2 on 1 Venetian (Right-Right)"})
                result.Add(New With {.Value = "2 on 1 Left-Right", .Text = "2 on 1 Venetian (Left-Right)"})
            End If

            If blindName = "Econo 50mm (Cordless)" Then
                result.Add(New With {.Value = "Single", .Text = "Single Venetian"})
            End If
        End If

        If type = "ValanceType" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" Then
                result.Add(New With {.Value = "75mm Valance", .Text = "75mm"})
                result.Add(New With {.Value = "89mm Valance", .Text = "89mm"})
            End If
            If blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" Then
                result.Add(New With {.Value = "76mm Valance", .Text = "76mm"})
            End If
            If blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm" Then
                result.Add(New With {.Value = "76mm Valance", .Text = "76mm"})
            End If
            If blindName = "Econo 50mm (Cordless)" Then
                result.Add(New With {.Value = "Standard", .Text = "Standard"})
            End If
        End If

        If type = "ValancePosition" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If Not String.IsNullOrEmpty(blindName) Then
                result.Add(New With {.Value = "Left", .Text = "Left"})
                result.Add(New With {.Value = "Right", .Text = "Right"})
                result.Add(New With {.Value = "Both Sides", .Text = "Both Sides"})

            End If
            If blindName = "Econo 50mm (Cordless)" Then
                result.Clear()
                result.Add(New With {.Value = "Left", .Text = "Left"})
                result.Add(New With {.Value = "Right", .Text = "Right"})
            End If
        End If

        If type = "LayoutCodeShutter" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If blindName = "Hinged" Then
                result.Add(New With {.Value = "L", .Text = "L"})
                result.Add(New With {.Value = "R", .Text = "R"})
                result.Add(New With {.Value = "LR", .Text = "LR"})
                result.Add(New With {.Value = "LD-R", .Text = "LD-R"})
                result.Add(New With {.Value = "L-DR", .Text = "L-DR"})
                result.Add(New With {.Value = "LTLR", .Text = "LTLR"})
                result.Add(New With {.Value = "LRTR", .Text = "LRTR"})
                result.Add(New With {.Value = "LRTLR", .Text = "LRTLR"})
                result.Add(New With {.Value = "LTLRTR", .Text = "LTLRTR"})
                result.Add(New With {.Value = "LD-RTLD-R", .Text = "LD-RTLD-R"})
                result.Add(New With {.Value = "L-DRTL-DR", .Text = "L-DRTL-DR"})
                result.Add(New With {.Value = "Other", .Text = "Other"})
            End If

            If blindName = "Hinged Bi-fold" Then
                result.Add(New With {.Value = "LL", .Text = "LL"})
                result.Add(New With {.Value = "RR", .Text = "RR"})
                result.Add(New With {.Value = "LLRR", .Text = "LLRR"})
                result.Add(New With {.Value = "Other", .Text = "Other"})
            End If

            If blindName = "Track Bi-fold" Then
                result.Add(New With {.Value = "LL", .Text = "LL"})
                result.Add(New With {.Value = "RR", .Text = "RR"})
                result.Add(New With {.Value = "LLRR", .Text = "LLRR"})
                result.Add(New With {.Value = "LLLL", .Text = "LLLL"})
                result.Add(New With {.Value = "RRRR", .Text = "RRRR"})
                result.Add(New With {.Value = "LLRRRR", .Text = "LLRRRR"})
                result.Add(New With {.Value = "LLLLRR", .Text = "LLLLRR"})
                result.Add(New With {.Value = "LLLLLL", .Text = "LLLLLL"})
                result.Add(New With {.Value = "RRRRRR", .Text = "RRRRRR"})
                result.Add(New With {.Value = "LLRRRRRR", .Text = "LLRRRRRR"})
                result.Add(New With {.Value = "LLLLRRRR", .Text = "LLLLRRRR"})
                result.Add(New With {.Value = "LLLLLLRR", .Text = "LLLLLLRR"})
                result.Add(New With {.Value = "LLLLLLLL", .Text = "LLLLLLLL"})
                result.Add(New With {.Value = "RRRRRRRR", .Text = "RRRRRRRR"})
                result.Add(New With {.Value = "Other", .Text = "Other"})
            End If

            If blindName = "Track Sliding" Then
                result.Add(New With {.Value = "BF", .Text = "BF"})
                result.Add(New With {.Value = "FB", .Text = "FB"})
                result.Add(New With {.Value = "BFB", .Text = "BFB"})
                result.Add(New With {.Value = "FBF", .Text = "FBF"})
                result.Add(New With {.Value = "BFFB", .Text = "BFFB"})
                result.Add(New With {.Value = "FBBF", .Text = "FBBF"})
                result.Add(New With {.Value = "BBFF", .Text = "BBFF"})
                result.Add(New With {.Value = "FFBB", .Text = "FFBB"})
                result.Add(New With {.Value = "Other", .Text = "Other"})
            End If

            If blindName = "Track Sliding Single Track" Then
                result.Add(New With {.Value = "F", .Text = "F"})
                result.Add(New With {.Value = "FF", .Text = "FF"})
                result.Add(New With {.Value = "FFF", .Text = "FFF"})
                result.Add(New With {.Value = "FFFF", .Text = "FFFF"})
                result.Add(New With {.Value = "Other", .Text = "Other"})
            End If

            If blindName = "Fixed" Then
                result.Add(New With {.Value = "", .Text = ""})
                result.Add(New With {.Value = "F", .Text = "F"})
                result.Add(New With {.Value = "FF", .Text = "FF"})
                result.Add(New With {.Value = "FFF", .Text = "FFF"})
                result.Add(New With {.Value = "FFFF", .Text = "FFFF"})
                result.Add(New With {.Value = "FFFFFF", .Text = "FFFFFF"})
                result.Add(New With {.Value = "Other", .Text = "Other"})
            End If
        End If

        If type = "FrameTypeShutter" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
                result.Add(New With {.Value = "Beaded L 48mm", .Text = "Beaded L 48mm"})
                result.Add(New With {.Value = "Insert L 50mm", .Text = "Insert L 50mm"})
                result.Add(New With {.Value = "Insert L 63mm", .Text = "Insert L 63mm"})
                If customtype = "Inside" Then
                    result.Add(New With {.Value = "Small Bullnose Z Frame", .Text = "Small Bullnose Z Frame"})
                    result.Add(New With {.Value = "Large Bullnose Z Frame", .Text = "Large Bullnose Z Frame"})
                    result.Add(New With {.Value = "Colonial Z Frame", .Text = "Colonial Z Frame"})
                End If
                result.Add(New With {.Value = "No Frame", .Text = "No Frame"})
            End If

            If blindName = "Track Bi-fold" Then
                result.Add(New With {.Value = "100mm", .Text = "100mm"})
                result.Add(New With {.Value = "160mm", .Text = "160mm"})
            End If

            If blindName = "Track Sliding" Then
                result.Add(New With {.Value = "100mm", .Text = "100mm"})
                result.Add(New With {.Value = "160mm", .Text = "160mm"})
                result.Add(New With {.Value = "200mm", .Text = "200mm"})
            End If

            If blindName = "Track Sliding Single Track" Then
                result.Add(New With {.Value = "100mm", .Text = "100mm"})
            End If

            If blindName = "Fixed" Then
                result.Add(New With {.Value = "U Channel", .Text = "U Channel"})
                result.Add(New With {.Value = "19x19 Light Block", .Text = "19x19 Light Block"})
            End If
        End If

        If type = "LeftFrameShutter" Then
            If customtype = "Beaded L 48mm" OrElse customtype = "Insert L 50mm" OrElse customtype = "Insert L 63mm" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "Sill Plate (9.5mm)", .Text = "Sill Plate (9.5mm)"})
            End If
            If customtype = "Small Bullnose Z Frame" OrElse customtype = "Large Bullnose Z Frame" OrElse customtype = "Colonial Bullnose Z Frame" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "Sill Plate (9.5mm)", .Text = "Sill Plate (9.5mm)"})
                result.Add(New With {.Value = "Sill Plate (Bullnose Z)", .Text = "Sill Plate (Bullnose Z)"})
            End If
            If customtype = "No Frame" Then
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
            End If
            If customtype = "100mm" OrElse customtype = "160mm" OrElse customtype = "200mm" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
            End If

            If customtype = "U Channel" Then
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "L Strip", .Text = "L Strip"})
            End If
            If customtype = "19x19 Light Block" Then
                result.Add(New With {.Value = "No", .Text = "No"})
            End If
        End If

        If type = "RightFrameShutter" Then
            If customtype = "Beaded L 48mm" OrElse customtype = "Insert L 50mm" OrElse customtype = "Insert L 63mm" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "Sill Plate (9.5mm)", .Text = "Sill Plate (9.5mm)"})
            End If
            If customtype = "Small Bullnose Z Frame" OrElse customtype = "Large Bullnose Z Frame" OrElse customtype = "Colonial Bullnose Z Frame" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "Sill Plate (9.5mm)", .Text = "Sill Plate (9.5mm)"})
                result.Add(New With {.Value = "Sill Plate (Bullnose Z)", .Text = "Sill Plate (Bullnose Z)"})
            End If
            If customtype = "No Frame" Then
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
            End If
            If customtype = "100mm" OrElse customtype = "160mm" OrElse customtype = "200mm" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
            End If

            If customtype = "U Channel" Then
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "L Strip", .Text = "L Strip"})
            End If
            If customtype = "19x19 Light Block" Then
                result.Add(New With {.Value = "No", .Text = "No"})
            End If
        End If

        If type = "TopFrameShutter" Then
            If customtype = "Beaded L 48mm" OrElse customtype = "Insert L 50mm" OrElse customtype = "Insert L 63mm" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "Sill Plate (9.5mm)", .Text = "Sill Plate (9.5mm)"})
                result.Add(New With {.Value = "L Striker Plate", .Text = "L Striker Plate"})
            End If
            If customtype = "Small Bullnose Z Frame" OrElse customtype = "Large Bullnose Z Frame" OrElse customtype = "Colonial Bullnose Z Frame" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "Sill Plate (9.5mm)", .Text = "Sill Plate (9.5mm)"})
                result.Add(New With {.Value = "Sill Plate (Bullnose Z)", .Text = "Sill Plate (Bullnose Z)"})
                result.Add(New With {.Value = "L Striker Plate", .Text = "L Striker Plate"})
            End If
            If customtype = "No Frame" Then
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "L Striker Plate", .Text = "L Striker Plate"})
            End If
            If customtype = "100mm" OrElse customtype = "160mm" OrElse customtype = "200mm" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
            End If

            If customtype = "U Channel" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
            End If
            If customtype = "19x19 Light Block" Then
                result.Add(New With {.Value = "No", .Text = "No"})
            End If
        End If

        If type = "BottomFrameShutter" Then
            If customtype = "Beaded L 48mm" OrElse customtype = "Insert L 50mm" OrElse customtype = "Insert L 63mm" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "Sill Plate (9.5mm)", .Text = "Sill Plate (9.5mm)"})
                result.Add(New With {.Value = "L Striker Plate", .Text = "L Striker Plate"})
            End If
            If customtype = "Small Bullnose Z Frame" OrElse customtype = "Large Bullnose Z Frame" OrElse customtype = "Colonial Bullnose Z Frame" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "Sill Plate (9.5mm)", .Text = "Sill Plate (9.5mm)"})
                result.Add(New With {.Value = "Sill Plate (Bullnose Z)", .Text = "Sill Plate (Bullnose Z)"})
                result.Add(New With {.Value = "L Striker Plate", .Text = "L Striker Plate"})
            End If
            If customtype = "No Frame" Then
                result.Add(New With {.Value = "Light Block", .Text = "Light Block"})
                result.Add(New With {.Value = "L Striker Plate", .Text = "L Striker Plate"})
            End If
            If customtype = "100mm" OrElse customtype = "160mm" OrElse customtype = "200mm" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
                result.Add(New With {.Value = "No", .Text = "No"})
            End If

            If customtype = "U Channel" Then
                result.Add(New With {.Value = "Yes", .Text = "Yes"})
            End If
            If customtype = "19x19 Light Block" Then
                result.Add(New With {.Value = "No", .Text = "No"})
            End If
        End If

        If type = "BottomTrackShutter" Then
            Dim blindName As String = orderClass.GetBlindName(blindtype)
            If blindName = "Track Bi-fold" OrElse blindName = "Track Sliding" OrElse blindName = "Track Sliding Single Track" Then
                If Not String.IsNullOrEmpty(customtype) Then
                    result.Add(New With {.Value = "M Track", .Text = "M Track"})
                    result.Add(New With {.Value = "U Track", .Text = "U Track"})
                    If customtype = "Yes" Then
                        result.Clear()
                        result.Add(New With {.Value = "U Track", .Text = "U Track"})
                    End If
                End If
            End If
        End If

        If type = "MidrailCriticalShutter" Then

        End If

        Return result
    End Function

    'PROCESS

    <WebMethod()>
    Public Shared Function AluminiumProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer : Dim widthb As Integer
        Dim drop As Integer : Dim dropb As Integer

        Dim controllength As Integer : Dim controllengthb As Integer
        Dim wandlength As Integer : Dim wandlengthb As Integer

        Dim linearmetre As Decimal : Dim linearmetreb As Decimal
        Dim squaremetre As Decimal : Dim squaremetreb As Decimal

        Dim markup As Integer

        Dim totalItems As Integer = 1

        Dim controlpositionb As String = String.Empty
        Dim tilterpositionb As String = String.Empty

        Dim blindName As String = String.Empty
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "ALUMINIUM TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "ALUMINIUM COLOUR IS REQUIRED !"
        If String.IsNullOrEmpty(data.subtype) Then Return "ALUMINIUM SUB TYPE IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        If width < 200 Then Return "MINIMUM WIDTH IS 200MM !"
        If data.subtype.Contains("2 on 1") AndAlso width < 300 Then Return "MINIMUM WIDTH IS 300MM !"
        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If width > 3010 Then Return "MAXIMUM WIDTH IS 3010MM !"
        End If

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
        If drop < 250 Then Return "MINIMUM DROP IS 250MM !"

        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If drop > 3200 Then Return "MAXIMUM DROP IS 3200MM !"
        End If

        If data.subtype = "Single" Then
            If String.IsNullOrEmpty(data.controlposition) Then Return "CONTROL POSITION IS REQUIRED !"
            If String.IsNullOrEmpty(data.tilterposition) Then Return "TILTER POSITION IS REQUIRED !"
        End If

        If data.subtype = "Single" Then
            If width > 250 AndAlso width <= 299 AndAlso data.controlposition = data.tilterposition Then
                Return "PLEASE USE OPPOSITE CONTROL AND TILTER POSITIONS!"
            End If
        End If

        If String.IsNullOrEmpty(data.controllength) Then
            If data.subtype.Contains("2 on 1") Then Return "FIRST CORD LENGTH IS REQUIRED !"
            Return "CORD LENGTH IS REQUIRED !"
        End If

        If data.controllength = "Custom" Then
            If String.IsNullOrEmpty(data.controllengthvalue) Then
                If data.subtype = "2 on 1 Left-Right" Then Return "FIRST CORD LENGTH VALUE IS REQUIRED !"
                Return "CORD LENGTH VALUE IS REQUIRED !"
            End If
            If Not Integer.TryParse(data.controllengthvalue, controllength) OrElse controllength < 0 Then
                If data.subtype = "2 on 1 Left-Right" Then Return "PLEASE CHECK YOUR FIRST CORD LENGTH ORDER !"
                Return "PLEASE CHECK YOUR CORD LENGTH ORDER !"
            End If
            Dim thisStandard As Integer = Math.Ceiling(drop * 2 / 3)
            If thisStandard < 450 Then thisStandard = 450

            If controllength < thisStandard Then
                If data.subtype = "2 on 1 Left-Right" Then Return String.Format("MINIMUM FIRST CORD LENGTH IS {0}MM !", thisStandard)
                Return String.Format("MINIMUM CORD LENGTH IS {0}MM !", thisStandard)
            End If
        End If

        If data.subtype = "Single" OrElse data.subtype = "2 on 1 Left-Left" OrElse data.subtype = "2 on 1 Left-Right" Then
            If String.IsNullOrEmpty(data.wandlength) Then
                If data.subtype = "2 on 1 Left-Right" Then Return "FIRST WAND LENGTH IS REQUIRED !"
                Return "WAND LENGTH IS REQUIRED !"
            End If

            If data.wandlength = "Custom" Then
                If String.IsNullOrEmpty(data.wandlengthvalue) Then
                    If data.subtype = "2 on 1 Left-Right" Then Return "FIRST WAND LENGTH VALUE IS REQUIRED !"
                    Return "WAND LENGTH VALUE IS REQUIRED !"
                End If
                If Not String.IsNullOrEmpty(data.wandlengthvalue) Then
                    If Not Integer.TryParse(data.wandlengthvalue, wandlength) OrElse wandlength < 0 Then
                        If data.subtype = "2 on 1 Left-Right" Then Return "PLEASE CHECK YOUR FIRST WAND LENGTH ORDER !"
                        Return "PLEASE CHECK YOUR WAND LENGTH ORDER !"
                    End If

                    Dim thisStandard As Integer = Math.Ceiling(drop * 2 / 3)
                    If thisStandard < 450 Then thisStandard = 450
                    If wandlength < thisStandard Then
                        If data.subtype = "2 on 1 Left-Right" Then Return String.Format("MINIMUM FIRST WAND LENGTH IS {0}MM !", thisStandard)
                        Return String.Format("MINIMUM WAND LENGTH IS {0}MM !", thisStandard)
                    End If
                End If
            End If
        End If

        If data.subtype.Contains("2 on 1") Then
            If String.IsNullOrEmpty(data.widthb) Then Return "SECOND WIDTH IS REQUIRED !"
            If Not Integer.TryParse(data.widthb, widthb) OrElse widthb <= 0 Then Return "PLEASE CHECK YOUR SECOND WIDTH ORDER !"

            If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
                If widthb < 300 Then Return "MINIMUM WIDTH FOR SECOND BLIND IS 300MM !"
                Dim totalWidth As Integer = width + widthb
                If totalWidth > 3010 Then Return "TOTAL WIDTH COULDN'T MORE THAN 3010MM !"
            End If

            If String.IsNullOrEmpty(data.dropb) Then Return "SECOND DROP IS REQUIRED !"
            If Not Integer.TryParse(data.dropb, dropb) OrElse dropb <= 0 Then Return "PLEASE CHECK YOUR SECOND DROP ORDER !"
            If dropb < 250 Then Return "MINIMUM DROP IS 250MM !"
            If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
                If dropb > 3200 Then Return "MAXIMUM DROP FOR SECOND DROP IS 3200MM !"
            End If

            If String.IsNullOrEmpty(data.controllengthb) Then Return "SECOND CORD LENGTH IS REQUIRED !"

            If data.controllengthb = "Custom" Then
                If String.IsNullOrEmpty(data.controllengthvalueb) Then Return "SECOND CORD LENGTH VALUL IS REQUIRED !"
                If Not Integer.TryParse(data.controllengthvalueb, controllengthb) OrElse controllengthb < 0 Then Return "PLEASE CHECK YOUR SECOND CORD LENGTH ORDER !"

                Dim thisStandard As Integer = Math.Ceiling(dropb * 2 / 3)
                If thisStandard < 450 Then thisStandard = 450
                If controllengthb < thisStandard Then Return String.Format("MINIMUM SECOND CORD LENGTH IS {0}MM !", thisStandard)
            End If
        End If

        If data.subtype = "2 on 1 Right-Right" OrElse data.subtype = "2 on 1 Left-Right" Then
            If String.IsNullOrEmpty(data.wandlengthb) Then Return "SECOND WAND LENGTH IS REQUIRED !"
            If data.wandlengthb = "Custom" Then
                If String.IsNullOrEmpty(data.wandlengthvalueb) Then Return "SECOND WAND LENGTH VALUL IS REQUIRED !"
                If Not String.IsNullOrEmpty(data.wandlengthvalueb) Then
                    If Not Integer.TryParse(data.wandlengthvalueb, wandlengthb) OrElse wandlengthb < 0 Then Return "PLEASE CHECK YOUR SECOND WAND LENGTH ORDER !"
                    Dim thisStandard As Integer = Math.Ceiling(dropb * 2 / 3)
                    If wandlengthb < thisStandard Then Return String.Format("MINIMUM SECOND WAND LENGTH IS {0}MM !", thisStandard)
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        linearmetre = width / 1000
        squaremetre = width * drop / 1000000

        If data.subtype = "Single" Then
            widthb = 0 : dropb = 0
            data.controllengthb = String.Empty : data.wandlengthb = String.Empty
            controllengthb = 0 : wandlengthb = 0
        End If

        If data.subtype = "2 on 1 Left-Left" Then
            data.controlposition = "Left" : data.tilterposition = "Left"
            controlpositionb = "Left" : tilterpositionb = String.Empty
            data.wandlengthb = String.Empty : wandlengthb = 0

            linearmetreb = widthb / 1000
            squaremetreb = widthb * dropb / 1000000

            totalItems = 2
        End If

        If data.subtype = "2 on 1 Right-Right" Then
            data.controlposition = "Right" : data.tilterposition = String.Empty
            controlpositionb = "Right" : tilterpositionb = "Right"
            data.wandlength = String.Empty : wandlength = 0

            linearmetreb = widthb / 1000
            squaremetreb = widthb * dropb / 1000000

            totalItems = 2
        End If

        If data.subtype = "2 on 1 Left-Right" Then
            data.controlposition = "Left" : data.tilterposition = "Right"
            controlpositionb = "Left" : tilterpositionb = "Right"

            linearmetreb = widthb / 1000
            squaremetreb = widthb * dropb / 1000000

            totalItems = 2
        End If

        If data.controllength = "Standard" Then
            controllength = Math.Ceiling(drop * 2 / 3)
            If controllength < 450 Then controllength = 450
        End If

        If data.controllengthb = "Standard" Then
            controllengthb = Math.Ceiling(drop * 2 / 3)
            If controllengthb < 450 Then controllengthb = 450
        End If

        If data.wandlength = "Standard" Then
            wandlength = Math.Ceiling(drop * 2 / 3)
            If wandlength < 450 Then wandlength = 450
        End If

        If data.wandlengthb = "Standard" Then
            wandlengthb = Math.Ceiling(drop * 2 / 3)
            If wandlengthb < 450 Then wandlengthb = 450
        End If

        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(blindName, data.designid)
        Dim priceProductGroupB As String = String.Empty

        If data.subtype.Contains("2 on 1") Then priceProductGroupB = priceProductGroup

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, PriceProductGroupIdB, SubType, Qty, Room, Mounting, ControlPosition, ControlPositionB, TilterPosition, TilterPositionB, Width, WidthB, [Drop], DropB, Supply, ControlLength, ControlLengthValue, ControlLengthB, ControlLengthValueB, WandLength, WandLengthValue, WandLengthB, WandLengthValueB, Notes, LinearMetre, LinearMetreB, SquareMetre, SquareMetreB, TotalItems, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @PriceProductGroupIdB, @SubType, 1, @Room, @Mounting, @ControlPosition, @ControlPositionB, @TilterPosition, @TilterPositionB, @Width, @WidthB, @Drop, @DropB, @Supply, @ControlLength, @ControlLengthValue, @ControlLengthB, @ControlLengthValueB, @WandLength, @WandLengthValue, @WandLengthB, @WandLengthValueB, @Notes, @LinearMetre, @LinearMetreB, @SquareMetre, @SquareMetreB, @TotalItems, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@SubType", data.subtype)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@TilterPosition", data.tilterposition)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                        myCmd.Parameters.AddWithValue("@WandLength", data.wandlength)
                        myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                        myCmd.Parameters.AddWithValue("@ControlPositionB", controlpositionb)
                        myCmd.Parameters.AddWithValue("@TilterPositionB", tilterpositionb)
                        myCmd.Parameters.AddWithValue("@Widthb", widthb)
                        myCmd.Parameters.AddWithValue("@DropB", dropb)
                        myCmd.Parameters.AddWithValue("@ControlLengthB", data.controllengthb)
                        myCmd.Parameters.AddWithValue("@ControlLengthValueB", controllengthb)
                        myCmd.Parameters.AddWithValue("@WandLengthB", data.wandlengthb)
                        myCmd.Parameters.AddWithValue("@WandLengthValueB", wandlengthb)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearmetre)
                        myCmd.Parameters.AddWithValue("@LinearMetreB", linearmetreb)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squaremetre)
                        myCmd.Parameters.AddWithValue("@SquareMetreB", squaremetreb)
                        myCmd.Parameters.AddWithValue("@Supply", data.supply)
                        myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, PriceProductGroupId=@PriceProductGroupId, PriceProductGroupIdB=@PriceProductGroupIdB, SubType=@Subtype, Qty=1, Room=@Room, Mounting=@Mounting, ControlPosition=@ControlPosition, ControlPositionB=@ControlPositionB, TilterPosition=@TilterPosition, TilterPositionB=@TilterPositionB, Width=@Width, WidthB=@WidthB, [Drop]=@Drop, DropB=@DropB, Supply=@Supply, ControlLength=@ControlLength, ControlLengthB=@ControlLengthB, ControlLengthValue=@ControlLengthValue, ControlLengthValueB=@ControlLengthValueB, WandLength=@WandLength, WandLengthB=@WandLengthB, WandLengthValue=@WandLengthValue, WandLengthValueB=@WandLengthValueB, Notes=@Notes, LinearMetre=@LinearMetre, LinearMetreB=@LinearMetreB, SquareMetre=@SquareMetre, SquareMetreB=@SquareMetreB, MarkUp=@MarkUp, TotalItems=@TotalItems, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@SubType", data.subtype)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@TilterPosition", data.tilterposition)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                    myCmd.Parameters.AddWithValue("@WandLength", data.wandlength)
                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                    myCmd.Parameters.AddWithValue("@ControlPositionB", controlpositionb)
                    myCmd.Parameters.AddWithValue("@TilterPositionB", tilterpositionb)
                    myCmd.Parameters.AddWithValue("@Widthb", widthb)
                    myCmd.Parameters.AddWithValue("@DropB", dropb)
                    myCmd.Parameters.AddWithValue("@ControlLengthB", data.controllengthb)
                    myCmd.Parameters.AddWithValue("@ControlLengthValueB", controllengthb)
                    myCmd.Parameters.AddWithValue("@WandLengthB", data.wandlengthb)
                    myCmd.Parameters.AddWithValue("@WandLengthValueB", wandlengthb)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearmetre)
                    myCmd.Parameters.AddWithValue("@LinearMetreB", linearmetreb)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squaremetre)
                    myCmd.Parameters.AddWithValue("@SquareMetreB", squaremetreb)
                    myCmd.Parameters.AddWithValue("@Supply", data.supply)
                    myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function CellularProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer : Dim widthb As Integer
        Dim drop As Integer : Dim dropb As Integer

        Dim clvalue As Integer
        Dim markup As Integer

        Dim linearMetre As Decimal : Dim linearMetreB As Decimal
        Dim squareMetre As Decimal : Dim squareMetreB As Decimal

        Dim totalItems As Integer = 1

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty
        Dim tubeName As String = String.Empty
        Dim controlName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)
        If Not String.IsNullOrEmpty(data.controltype) Then controlName = orderClass.GetControlName(data.controltype)

        Dim factory As String = orderClass.GetFabricFactory(data.fabriccolour)
        Dim factoryB As String = orderClass.GetFabricFactory(data.fabriccolourb)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "CELLULAR TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.controltype) Then Return "CONTROL TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+ !"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"

        If blindName = "Day & Night" Then
            If String.IsNullOrEmpty(data.fabrictypeb) Then Return "FABRIC TYPE FOR DAY & NIGHT IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolourb) Then Return "FABRIC COLOUR FOR DAY & NIGHT IS REQUIRED !"
            If Not factory = factoryB Then
                If factory = "Regular" Then
                    Return "THE FIRST FABRIC SELECTED IS <b>REGULAR</b>.<br />PLEASE CHOOSE <b>REGULAR</b> AS WELL FOR THE SECOND FABRIC. !"
                End If
                Return "THE FIRST FABRIC SELECTED IS <b>EXPRESS</b>.<br />PLEASE CHOOSE <b>EXPRESS</b> AS WELL FOR THE SECOND FABRIC. !"
            End If
        End If

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If controlName = "Corded" Then
                If blindName = "Standard" AndAlso width < 200 Then Return "MINIMUM WIDTH IS 200MM !"
                If blindName = "TDBU" AndAlso width < 400 Then Return "MINIMUM WIDTH IS 400MM !"
                If blindName = "TDBU" AndAlso width < 450 Then Return "MINIMUM WIDTH IS 450MM !"
                If width > 3000 Then Return "MAXIMUM WIDTH IS 3000MM !"
            End If
            If controlName = "Cordless" Then
                If blindName = "Standard" AndAlso width < 400 Then Return "MINIMUM WIDTH IS 400MM !"
                If blindName = "TDBU" AndAlso width < 700 Then Return "MINIMUM WIDTH IS 700MM !"
                If blindName = "TDBU" AndAlso width < 700 Then Return "MINIMUM WIDTH IS 700MM !"
                If width > 2400 Then Return "MAXIMUM WIDTH IS 2400MM !"
            End If
        End If

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEAS CHECK YOUR DROP ORDER !"

        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If controlName = "Corded" Then
                If blindName = "Standard" AndAlso drop < 300 Then Return "MINIMUM DROP IS 300MM !"
                If blindName = "TDBU" AndAlso drop < 400 Then Return "MINIMUM DROP IS 400MM !"
                If blindName = "Day & Night" AndAlso drop < 750 Then Return "MINIMUM DROP IS 750MM !"
                If drop > 3600 Then Return "MAXIMUM DROP IS 3600MM !"
            End If
            If controlName = "Cordless" Then
                If blindName = "Standard" AndAlso drop < 400 Then Return "MINIMUM DROP IS 400MM !"
                If blindName = "TDBU" AndAlso drop < 450 Then Return "MINIMUM DROP IS 450MM !"
                If blindName = "Day & Night" AndAlso drop < 750 Then Return "MINIMUM DROP IS 750MM !"
                If drop > 2100 Then Return "MAXIMUM DROP IS 2100MM !"
            End If
        End If

        If controlName = "Corded" AndAlso blindName = "Standard" Then
            If String.IsNullOrEmpty(data.controlposition) Then Return "CORD POSITION IS REQUIRED!"
        End If

        If controlName = "Corded" Then
            If String.IsNullOrEmpty(data.controllength) Then Return "CORD LENGTH IS REQUIRED !"
            If data.controllength = "Custom" Then
                If String.IsNullOrEmpty(data.controllengthvalue) Then Return "CORD LENGTH VALUE IS REQUIRED !"
                If Not Integer.TryParse(data.controllengthvalue, clvalue) OrElse clvalue < 0 Then Return "PLEASE CHECK YOUR CORD LENGTH ORDER !"
                Dim stdLength As Integer = Math.Ceiling(drop * 2 / 3)
                If clvalue < stdLength Then Return String.Format("MINIMUM CORD LENGTH IS {0}MM !", stdLength)
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+ !"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        linearMetre = width / 1000
        squareMetre = width * drop / 1000000

        If blindName = "Standard" OrElse blindName = "Top Down Bottom Up" Then
            widthb = 0 : dropb = 0
            data.fabrictypeb = String.Empty : data.fabriccolourb = String.Empty
        End If

        If blindName = "Day & Night" Then
            widthb = width : dropb = drop
            linearMetreB = width / 1000
            squareMetreB = widthb * dropb / 1000000
            totalItems = 2
        End If

        If blindName = "Day & Night" OrElse blindName = "Top Down Bottom Up" Then
            data.controlposition = "Both Sides"
        End If

        If controlName = "Corded" AndAlso data.controllength = "Standard" Then
            clvalue = Math.Ceiling(drop * 2 / 3)
        End If

        If controlName = "Cordless" Then
            data.controlposition = String.Empty : data.controllength = String.Empty : clvalue = 0
        End If

        Dim fabricGroup As String = orderClass.GetFabricGroup(data.fabrictype)

        Dim groupName As String = String.Format("{0} - {1} - {2} - {3}", blindName, controlName, fabricGroup, factory)

        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)
        Dim priceProductGroupB As String = String.Empty

        If blindName = "Day & Night" Then
            groupName = String.Format("{0} - {1} - {2}", blindName, controlName, factory)
            Dim groupNameB As String = String.Format("{0} - {1} - {2}", blindName, controlName, factoryB)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupNameB, data.designid)
        End If

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricIdB, FabricColourId, FabricColourIdB, PriceProductGroupId, PriceProductGroupIdB, Qty, Room, Mounting, Width, WidthB, [Drop], DropB, ControlPosition, ControlLength, ControlLengthValue, Supply, LinearMetre, LinearMetreB, SquareMetre, SquareMetreB, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricIdB, @FabricColourId, @FabricColourIdB, @PriceProductGroupId, @PriceProductGroupIdB, @Qty, @Room, @Mounting, @Width, @WidthB, @Drop, @DropB, @ControlPosition, @ControlLength, @ControlLengthValue, @Supply, @LinearMetre, @LinearMetreB, @SquareMetre, @SquareMetreB, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@FabricId", data.fabrictype)
                        myCmd.Parameters.AddWithValue("@FabricColourId", data.fabriccolour)
                        myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabrictypeb), CType(DBNull.Value, Object), data.fabrictypeb))
                        myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(data.fabriccolourb), CType(DBNull.Value, Object), data.fabriccolourb))
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@WidthB", widthb)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@DropB", dropb)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", clvalue)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                        myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                        myCmd.Parameters.AddWithValue("@Supply", data.supply)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricIdB=@FabricIdB, FabricColourId=@FabricColourId, FabricColourIdB=@FabricColourIdB,  PriceProductGroupId=@PriceProductGroupId, PriceProductGroupIdB=@PriceProductGroupIdB, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, WidthB=@WidthB, [Drop]=@Drop, DropB=@DropB, ControlPosition=@ControlPosition, ControlLength=@ControlLength, ControlLengthValue=@ControlLengthValue, Supply=@Supply, LinearMetre=@LinearMetre, LinearMetreB=@LinearMetreB, SquareMetre=@SquareMetre, SquareMetreB=@SquareMetreB, TotalItems=@TotalItems, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@FabricId", data.fabrictype)
                    myCmd.Parameters.AddWithValue("@FabricColourId", data.fabriccolour)
                    myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabrictypeb), CType(DBNull.Value, Object), data.fabrictypeb))
                    myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(data.fabriccolourb), CType(DBNull.Value, Object), data.fabriccolourb))
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@WidthB", widthb)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@DropB", dropb)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", clvalue)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                    myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                    myCmd.Parameters.AddWithValue("@Supply", data.supply)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function CurtainProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer
        Dim widthb As Integer
        Dim drop As Integer
        Dim dropb As Integer
        Dim controllength As Integer
        Dim controllengthb As Integer
        Dim returnlength As Integer
        Dim returnlengthb As Integer

        Dim linearmetre As Decimal
        Dim linearmetreb As Decimal
        Dim squaremetre As Decimal
        Dim squaremetreb As Decimal

        Dim totalitems As Integer = 1

        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty
        Dim tubeName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)
        If Not String.IsNullOrEmpty(data.tubetype) Then orderClass.GetTubeName(data.tubetype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)
        Dim roleName As String = orderClass.GetUserRoleName(data.loginid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "CURTAIN TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If

        If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" OrElse blindName = "Curtain Only" Then
            If String.IsNullOrEmpty(data.mounting) Then Return "FITTING IS REQUIRED !"
        End If

        If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" OrElse blindName = "Curtain Only" Then
            If String.IsNullOrEmpty(data.heading) Then Return "CURTAIN HEADING IS REQUIRED !"
        End If

        If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" OrElse blindName = "Curtain Only" OrElse blindName = "Fabric Only" Then
            If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"
        End If

        If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" OrElse blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.tracktype) Then Return "TRACK TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.trackcolour) Then Return "TRACK COLOUR IS REQUIRED !"
            If String.IsNullOrEmpty(data.trackdraw) Then Return "TRACK DRAW IS REQUIRED !"
            If String.IsNullOrEmpty(data.stackposition) Then Return "STACK POSITION IS REQUIRED !"
        End If

        If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" OrElse blindName = "Track Only" OrElse blindName = "Curtain Only" Then
            If String.IsNullOrEmpty(data.stackposition) Then Return "STACK POSITION IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" OrElse blindName = "Curtain Only" OrElse blindName = "Track Only" Then
            If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
                If width > 6000 Then Return "MAXIMUM WIDTH IS 6000MM !"
            End If
        End If

        If blindName = "Fabric Only" Then
            If width > 2800 Then Return "MAXIMUM WIDTH IS 2800MM !"
        End If

        If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" OrElse blindName = "Curtain Only" OrElse blindName = "Fabric Only" Then
            If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
            If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
                If drop > 4000 Then Return "MAXIMUM DROP IS 4000MM !"
            End If
        End If

        If data.trackdraw = "Flick Stick" Then
            If String.IsNullOrEmpty(data.controlcolour) Then Return "CONTROL COLOUR IS REQUIRED !"
            If String.IsNullOrEmpty(data.controllength) Then Return "CONTROL LENGTH IS REQUIRED !"
            If Not Integer.TryParse(data.controllength, controllength) OrElse controllength <= 0 Then Return "PLEASE CHECK YOUR CONTROL LENGTH ORDER !"
        End If

        If blindName = "Double Curtain & Track" Then
            If String.IsNullOrEmpty(data.headingb) Then Return "SECOND CURTAIN HEADING IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabrictypeb) Then Return "SECOND FABRIC TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolourb) Then Return "SECOND FABRIC COLOUR IS REQUIRED !"
            If String.IsNullOrEmpty(data.tracktypeB) Then Return "SECOND TRACK TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.trackcolourb) Then Return "SECOND TRACK COLOUR IS REQUIRED !"
            If String.IsNullOrEmpty(data.trackdrawb) Then Return "SECOND TRACK DRAW IS REQUIRED !"
            If String.IsNullOrEmpty(data.stackpositionb) Then Return "SECOND STACK POSITION IS REQUIRED !"

            If data.trackdraw = "Flick Stick" Then
                If String.IsNullOrEmpty(data.controlcolourb) Then Return "SECOND CONTROL COLOUR IS REQUIRED !"
                If String.IsNullOrEmpty(data.controllengthb) Then Return "SECOND CONTROL LENGTH IS REQUIRED !"
                If Not Integer.TryParse(data.controllengthb, controllengthb) OrElse controllengthb <= 0 Then Return "PLEASE CHECK YOUR SECOND CONTROL LENGTH ORDER !"
            End If

            If String.IsNullOrEmpty(data.widthb) Then Return "SECOND WIDTH IS REQUIRED !"
            If Not Integer.TryParse(data.widthb, widthb) OrElse widthb <= 0 Then Return "PLEASE CHECK YOUR SECOND WIDTH WIDTH ORDER !"

            If String.IsNullOrEmpty(data.dropb) Then Return "SECOND DROP IS REQUIRED !"
            If Not Integer.TryParse(data.dropb, dropb) OrElse dropb <= 0 Then Return "PLEASE CHECK YOUR SECOND DROP ORDER !"
        End If

        If Not String.IsNullOrEmpty(data.returnlengthvalue) Then
            If Not Integer.TryParse(data.returnlengthvalue, returnlength) OrElse returnlength <= 0 Then Return "PLEASE CHECK YOUR RETURN LENGTH (LEFT) ORDER !"
        End If
        If Not String.IsNullOrEmpty(data.returnlengthvalueb) Then
            If Not Integer.TryParse(data.returnlengthvalueb, returnlengthb) OrElse returnlengthb <= 0 Then Return "PLEASE CHECK YOUR RETURN LENGTH (RIGHT) ORDER !"
        End If

        If blindName = "Single Curtain & Track" OrElse blindName = "Double Curtain & Track" OrElse blindName = "Curtain Only" Then
            If String.IsNullOrEmpty(data.bottomhem) Then Return "BOTTOM HEM IS REQUIRED !"
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If blindName = "Fabric Only" Then
            data.heading = String.Empty : data.headingb = String.Empty
            data.fabrictypeb = String.Empty : data.fabriccolourb = String.Empty
            data.tracktype = String.Empty : data.trackcolour = String.Empty
            data.tracktypeB = String.Empty : data.trackcolourb = String.Empty
            data.trackdraw = String.Empty : data.trackdrawb = String.Empty
            data.controlcolour = String.Empty : controllength = 0
            data.controlcolourb = String.Empty : controllengthb = 0
            data.stackposition = String.Empty : data.stackpositionb = String.Empty
            widthb = 0 : dropb = 0

            linearmetre = width / 1000
            squaremetre = Math.Ceiling(width * drop / 1000000)
        End If

        If blindName = "Single Curtain & Track" Then
            data.headingb = String.Empty
            data.fabrictypeb = String.Empty : data.fabriccolourb = String.Empty
            data.tracktypeB = String.Empty : data.trackcolourb = String.Empty
            data.trackdrawb = String.Empty
            data.stackpositionb = String.Empty
            If Not data.trackdraw = "Flick Stick" Then
                data.controlcolour = String.Empty : controllength = 0
            End If
            data.controlcolourb = String.Empty : controllengthb = 0
            widthb = 0 : dropb = 0

            linearmetre = width / 1000
            squaremetre = Math.Ceiling(width * drop / 1000000)
        End If

        If blindName = "Double Curtain & Track" Then
            totalitems = 2

            linearmetre = width / 1000
            squaremetre = Math.Ceiling(width * drop / 1000000)

            linearmetreb = widthb / 1000
            squaremetreb = Math.Ceiling(widthb * dropb / 1000000)
        End If

        If blindName = "Curtain Only" Then
            data.headingb = String.Empty
            data.fabrictypeb = String.Empty : data.fabriccolourb = String.Empty
            data.tracktype = String.Empty : data.trackcolour = String.Empty
            data.tracktypeB = String.Empty : data.trackcolourb = String.Empty
            data.trackdraw = String.Empty : data.trackdrawb = String.Empty
            data.controlcolour = String.Empty : controllength = 0
            data.controlcolourb = String.Empty : controllengthb = 0
            data.stackpositionb = String.Empty
            widthb = 0 : dropb = 0

            linearmetre = width / 1000
            squaremetre = Math.Ceiling(width * drop / 1000000)
        End If

        If blindName = "Track Only" Then
            data.heading = String.Empty : data.headingb = String.Empty
            data.fabrictype = String.Empty : data.fabriccolour = String.Empty
            data.fabrictypeb = String.Empty : data.fabriccolourb = String.Empty
            data.tracktypeB = String.Empty : data.trackcolourb = String.Empty
            data.trackdrawb = String.Empty
            data.stackpositionb = String.Empty
            If Not data.trackdraw = "Flick Stick" Then
                data.controlcolour = String.Empty : controllength = 0
            End If
            data.controlcolourb = String.Empty : controllengthb = 0
            drop = 0 : widthb = 0 : dropb = 0

            linearmetre = width / 1000
        End If





        Dim priceProductGroup As String = String.Empty 'orderClass.GetPriceProductGroupId(groupName, data.designid)
        Dim priceProductGroupB As String = String.Empty

        Dim priceAdditional As String = String.Empty
        Dim priceAdditionalB As String = String.Empty

        If data.companyid = "2" Then
            Dim sellName As String = designName
            Dim groupFabric As String = orderClass.GetFabricGroup(data.fabrictype)
            Dim groupFabricB As String = orderClass.GetFabricGroup(data.fabrictypeb)

            Dim groupName As String = String.Format("{0} - {1}", designName, groupFabric)
            Dim groupNameB As String = String.Format("{0} - {1}", designName, groupFabricB)
            Dim trackGroupName As String = String.Format("{0} - {1}", designName, data.tracktype)
            Dim trackGroupNameB As String = String.Format("{0} - {1}", designName, data.tracktypeB)

            If blindName = "Curtain Only" Then
                groupNameB = String.Empty
                trackGroupName = String.Empty
                trackGroupNameB = String.Empty
            End If

            If blindName = "Single Curtain & Track" Then
                groupNameB = String.Empty
                trackGroupNameB = String.Empty
            End If

            If blindName = "Track Only" Then
                groupName = String.Format("{0} - {1}", designName, data.tracktype)
                groupNameB = String.Empty
                trackGroupName = String.Empty
                trackGroupNameB = String.Empty
            End If

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupNameB, data.designid)
            priceAdditional = orderClass.GetPriceProductGroupId(trackGroupName, data.designid)
            priceAdditionalB = orderClass.GetPriceProductGroupId(trackGroupNameB, data.designid)
        End If

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricIdB, FabricColourId, FabricColourIdB, PriceProductGroupId, PriceProductGroupIdB, PriceAdditional, PriceAdditionalB, Qty, Room, Mounting, Width, WidthB, [Drop], DropB, Heading, HeadingB, TrackType, TrackTypeB, TrackColour, TrackColourB, TrackDraw, TrackDrawB, StackPosition, StackPositionB, ControlColour, ControlColourB, ControlLengthValue, ControlLengthValueB, ReturnLengthValue, ReturnLengthValueB, BottomHem, Supply, LinearMetre, LinearMetreB, SquareMetre, SquareMetreB, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricIdB, @FabricColourId, @FabricColourIdB, @PriceProductGroupId, @PriceProductGroupIdB, @PriceAdditional, @PriceAdditionalB, @Qty, @Room, @Mounting, @Width, @WidthB, @Drop, @DropB, @Heading, @HeadingB, @TrackType, @TrackTypeB, @TrackColour, @TrackColourB, @TrackDraw, @TrackDrawB, @StackPosition, @StackPositionB, @ControlColour, @ControlColourB, @ControlLengthValue, @ControlLengthValueB, @ReturnLengthValue, @ReturnLengthValueB, @BottomHem, @Supply, @LinearMetre, @LinearMetre, @SquareMetre, @SquareMetreB, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                        myCmd.Parameters.AddWithValue("@PriceAdditional", If(String.IsNullOrEmpty(priceAdditional), CType(DBNull.Value, Object), priceAdditional))
                        myCmd.Parameters.AddWithValue("@PriceAdditionalB", If(String.IsNullOrEmpty(priceAdditionalB), CType(DBNull.Value, Object), priceAdditionalB))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabrictypeb), CType(DBNull.Value, Object), data.fabrictypeb))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(data.fabriccolourb), CType(DBNull.Value, Object), data.fabriccolourb))

                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)

                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@WidthB", widthb)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@DropB", dropb)
                        myCmd.Parameters.AddWithValue("@Heading", data.heading)
                        myCmd.Parameters.AddWithValue("@HeadingB", data.headingb)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@TrackTypeB", data.tracktypeB)
                        myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                        myCmd.Parameters.AddWithValue("@TrackColourB", data.trackcolourb)
                        myCmd.Parameters.AddWithValue("@TrackDraw", data.trackdraw)
                        myCmd.Parameters.AddWithValue("@TrackDrawB", data.trackdrawb)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                        myCmd.Parameters.AddWithValue("@StackPositionB", data.stackpositionb)
                        myCmd.Parameters.AddWithValue("@ControlColour", data.controlcolour)
                        myCmd.Parameters.AddWithValue("@ControlColourB", data.controlcolourb)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValueB", controllengthb)
                        myCmd.Parameters.AddWithValue("@ReturnLengthValue", returnlength)
                        myCmd.Parameters.AddWithValue("@ReturnLengthValueB", returnlengthb)
                        myCmd.Parameters.AddWithValue("@BottomHem", data.bottomhem)
                        myCmd.Parameters.AddWithValue("@Supply", data.tieback)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearmetre)
                        myCmd.Parameters.AddWithValue("@LinearMetreB", linearmetreb)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squaremetre)
                        myCmd.Parameters.AddWithValue("@SquareMetreB", squaremetreb)
                        myCmd.Parameters.AddWithValue("@TotalItems", totalitems)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricIdB=@FabricIdB, FabricColourId=@FabricColourId, FabricColourIdB=@FabricColourIdB, PriceProductGroupId=@PriceProductGroupId, PriceProductGroupIdB=@PriceProductGroupIdB, PriceAdditional=@PriceAdditional, PriceAdditionalB=@PriceAdditionalB, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, WidthB=@WidthB, [Drop]=@Drop, DropB=@DropB, Heading=@Heading, HeadingB=@HeadingB, TrackType=@TrackType, TrackTypeB=@TrackTypeB, TrackColour=@TrackColour, TrackColourB=@TrackColourB, TrackDraw=@TrackDraw, TrackDrawB=@TrackDrawB, StackPosition=@StackPosition, StackPositionB=@StackPositionB, ControlColour=@ControlColour, ControlColourB=@ControlColourB, ControlLengthValue=@ControlLengthValue, ControlLengthValueB=@ControlLengthValueB, ReturnLengthValue=@ReturnLengthValue, ReturnLengthValueB=@ReturnLengthValue, BottomHem=@BottomHem, Supply=@Supply, LinearMetre=@LinearMetre, LinearMetreB=@LinearMetreB, SquareMetre=@SquareMetre, SquareMetreB=@SquareMetreB, TotalItems=@TotalItems, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                    myCmd.Parameters.AddWithValue("@PriceAdditional", If(String.IsNullOrEmpty(priceAdditional), CType(DBNull.Value, Object), priceAdditional))
                    myCmd.Parameters.AddWithValue("@PriceAdditionalB", If(String.IsNullOrEmpty(priceAdditionalB), CType(DBNull.Value, Object), priceAdditionalB))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabrictypeb), CType(DBNull.Value, Object), data.fabrictypeb))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(data.fabriccolourb), CType(DBNull.Value, Object), data.fabriccolourb))

                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)

                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@WidthB", widthb)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@DropB", dropb)
                    myCmd.Parameters.AddWithValue("@Heading", data.heading)
                    myCmd.Parameters.AddWithValue("@HeadingB", data.headingb)
                    myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                    myCmd.Parameters.AddWithValue("@TrackTypeB", data.tracktypeB)
                    myCmd.Parameters.AddWithValue("@TrackColour", data.trackcolour)
                    myCmd.Parameters.AddWithValue("@TrackColourB", data.trackcolourb)
                    myCmd.Parameters.AddWithValue("@TrackDraw", data.trackdraw)
                    myCmd.Parameters.AddWithValue("@TrackDrawB", data.trackdrawb)
                    myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                    myCmd.Parameters.AddWithValue("@StackPositionB", data.stackpositionb)
                    myCmd.Parameters.AddWithValue("@ControlColour", data.controlcolour)
                    myCmd.Parameters.AddWithValue("@ControlColourB", data.controlcolourb)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValueB", controllengthb)
                    myCmd.Parameters.AddWithValue("@ReturnLengthValue", returnlength)
                    myCmd.Parameters.AddWithValue("@ReturnLengthValueB", returnlengthb)
                    myCmd.Parameters.AddWithValue("@BottomHem", data.bottomhem)
                    myCmd.Parameters.AddWithValue("@Supply", data.tieback)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearmetre)
                    myCmd.Parameters.AddWithValue("@LinearMetreB", linearmetreb)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squaremetre)
                    myCmd.Parameters.AddWithValue("@SquareMetreB", squaremetreb)
                    myCmd.Parameters.AddWithValue("@TotalItems", totalitems)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function DesignShadesProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer
        Dim drop As Integer
        Dim controllength As Integer
        Dim wandlength As Integer
        Dim markup As Integer

        Dim linearMetre As Decimal
        Dim squareMetre As Decimal

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty
        Dim controlName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)
        If Not String.IsNullOrEmpty(data.controltype) Then controlName = orderClass.GetControlName(data.controltype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "PLEASE CONTACT CUSTOMER SERVICE !"
        If String.IsNullOrEmpty(data.controltype) Then Return "CONTROL TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "TRACK COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If width < 500 Then Return "MINIMUM WIDTH IS 500MM !"
            If width > 2910 Then Return "MAXIMUM WIDTH IS 2910MM !"
        End If

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If drop < 600 Then Return "MINIMUM DROP IS 600MM !"
            If drop > 2700 Then Return "MINIMUM DROP IS 2700MM !"
        End If

        If String.IsNullOrEmpty(data.stackposition) Then Return "STACK POSITION IS REQUIRED !"
        If controlName = "Chain" AndAlso String.IsNullOrEmpty(data.controlposition) Then Return "CONTROL POSITION IS REQUIRED !"

        If controlName = "Chain" AndAlso String.IsNullOrEmpty(data.chaincolour) Then
            Return "CHAIN COLOUR IS REQUIRED !"
        End If

        If controlName = "Wand" AndAlso String.IsNullOrEmpty(data.wandcolour) Then
            Return "WAND COLOUR IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.controllength) Then
            If controlName = "Wand" Then Return "WAND LENGTH IS REQUIRED !"
            Return "CHAIN LENGTH IS REQUIRED !"
        End If

        If data.controllength = "Custom" Then
            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.chainlengthvalue) Then Return "CHAIN LENGTH VALUE IS REQUIRED !"
                If Not Integer.TryParse(data.chainlengthvalue, controllength) OrElse controllength < 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE ORDER !"

                Dim thisStandard As Integer = Math.Ceiling(drop * 2 / 3)
                If controllength < thisStandard Then
                    If controlName = "Chain" Then
                        Dim thisAlert As Integer = 750
                        If thisStandard > 750 Then thisAlert = 1000
                        If thisStandard > 1000 Then thisAlert = 1200
                        If thisStandard > 1200 Then thisAlert = 1500
                        Return String.Format("MINIMUM CONTROL LENGTH IS {0}MM !", thisAlert)
                    End If
                    Return String.Format("MINIMUM CONTROL LENGTH IS {0}MM !", controllength)
                End If
            End If

            If controlName = "Wand" Then
                If String.IsNullOrEmpty(data.wandlengthvalue) Then Return "WAND LENGTH VALUE IS REQUIRED !"
                If Not Integer.TryParse(data.wandlengthvalue, controllength) OrElse controllength < 0 Then Return "PLEASE CHECK YOUR WAND LENGTH VALUE ORDER !"
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If controlName = "Chain" Then
            data.wandcolour = String.Empty
            wandlength = 0
            If data.controllength = "Standard" Then
                controllength = 550
                Dim thisFormula As Integer = Math.Ceiling(drop * 2 / 3)
                If thisFormula > 500 Then controllength = 750
                If thisFormula > 750 Then controllength = 1000
                If thisFormula > 1000 Then controllength = 1200
                If thisFormula > 1200 Then controllength = 1500
            End If
        End If

        If controlName = "Wand" Then
            data.chaincolour = String.Empty
            If data.controllength = "Standard" Then controllength = Math.Ceiling(drop * 2 / 3)

            If data.stackposition = "Stack Right" Then data.controlposition = "Left"
            If data.stackposition = "Stack Left" Then data.controlposition = "Right"
            If data.stackposition = "Stack Split" Then data.controlposition = "Middle"
            If data.stackposition = "Stack Centre" Then data.controlposition = "Left and Right"
        End If

        linearMetre = width / 1000
        squareMetre = width * drop / 1000000

        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(designName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], StackPosition, ControlPosition, ControlLength, ControlLengthValue, WandColour, WandLengthValue, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @StackPosition, @ControlPosition, @ControlLength, @ControlLengthValue, @WandColour, @WandLengthValue, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@FabricId", data.fabrictype)
                        myCmd.Parameters.AddWithValue("@FabricColourId", data.fabriccolour)
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, ChainId=@ChainId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, StackPosition=@StackPosition, ControlPosition=@ControlPosition, ControlLength=@ControlLength, ControlLengthValue=@ControlLengthValue, WandColour=@WandColour, WandLengthValue=@WandLengthValue, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, TotalItems=1, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@FabricId", data.fabrictype)
                    myCmd.Parameters.AddWithValue("@FabricColourId", data.fabriccolour)
                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                    myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function LineaProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer
        Dim drop As Integer = 0
        Dim rlValue As Integer
        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim tubeName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "BLIND TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.tubetype) Then Return "VALANCE TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "COLOUR TYPE IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If data.fabricinsert = "Yes" Then
            If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If width < 600 Then Return "MINIMUM WIDTH IS 600MM !"
            If width > 2910 Then Return "MAXIMUM WIDTH IS 2910MM !"
        End If

        If String.IsNullOrEmpty(data.brackettype) Then Return "BRACKET TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.isblindin) Then Return "IS BLIND IN IS REQUIRED !"

        If Not String.IsNullOrEmpty(data.returnposition) AndAlso String.IsNullOrEmpty(data.returnlength) Then
            Return "RETURN LENGTH IS REQUIRED !"
        End If

        If data.returnlength = "Custom" Then
            If String.IsNullOrEmpty(data.returnlengthvalue) Then Return "RETURN LENGTH VALUE IS REQUIRED !"
            If Not Integer.TryParse(data.returnlengthvalue, rlValue) OrElse rlValue < 0 Then Return "PLEASE CHECK YOUR RETURN LENGTH VALUE ORDER !"
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If String.IsNullOrEmpty(data.fabricinsert) Then
            data.fabrictype = String.Empty : data.fabriccolour = String.Empty
        End If

        If data.returnlength = "Standard" Then
            If data.brackettype = "Single Roller" Then rlValue = 120
            If data.brackettype = "Dual Roller" Then
                rlValue = 145
                If width > 2410 Then rlValue = 180
            End If
            If data.brackettype = "Panel Screen - 3 Tracks" OrElse data.brackettype = "Panel Screen - 4 Tracks" Then
                rlValue = 160
            End If
            If data.brackettype = "Panel Screen - 5 Tracks" Then rlValue = 170
            If data.brackettype = "Vertical" Then rlValue = 120
            If data.brackettype = "Vertical with Extention" Then rlValue = 190
        End If

        Dim linearMetre As Decimal = width / 1000

        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(tubeName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId,PriceProductGroupId, FabricInsert, Qty, Room, Mounting, Width, [Drop], BracketType, IsBlindIn, ReturnPosition, ReturnLength, ReturnLengthValue, LinearMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @PriceProductGroupId, @FabricInsert, @Qty, @Room, @Mounting, @Width, 0, @BracketType, @IsBlindIn, @ReturnPosition, @ReturnLength, @ReturnLengthValue, @LinearMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@FabricInsert", data.fabricinsert)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@BracketType", data.brackettype)
                        myCmd.Parameters.AddWithValue("@IsBlindIn", data.isblindin)
                        myCmd.Parameters.AddWithValue("@ReturnPosition", data.returnposition)
                        myCmd.Parameters.AddWithValue("@ReturnLength", data.returnlength)
                        myCmd.Parameters.AddWithValue("@ReturnLengthValue", rlValue)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, PriceProductGroupId=@PriceProductGroupId, FabricInsert=@FabricInsert, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=0, BracketType=@BracketType, IsBlindIn=@IsBlindIn, ReturnPosition=@ReturnPosition, ReturnLength=@ReturnLength, ReturnLengthValue=@ReturnLengthValue, LinearMetre=@LinearMetre, TotalItems=1, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@FabricInsert", data.fabricinsert)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@BracketType", data.brackettype)
                    myCmd.Parameters.AddWithValue("@IsBlindIn", data.isblindin)
                    myCmd.Parameters.AddWithValue("@ReturnPosition", data.returnposition)
                    myCmd.Parameters.AddWithValue("@ReturnLength", data.returnlength)
                    myCmd.Parameters.AddWithValue("@ReturnLengthValue", rlValue)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function PanelGlideProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer
        Dim drop As Integer
        Dim wandcolour As String = String.Empty
        Dim wandlength As Integer
        Dim panelQty As Integer
        Dim markup As Integer

        Dim linearMetre As Decimal
        Dim squareMetre As Decimal

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty
        Dim tubeName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "PANEL SYSTEM IS REQUIRED !"
        If String.IsNullOrEmpty(data.tubetype) Then Return "PANEL STYLE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "TRACK COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"
        If qty > 5 Then Return "MAXIMUM QTY ORDER IS 5 !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If

        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Panel Only" Then
            If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If width < 610 Then Return "MINIMUM WIDTH IS 610MM !"
            If width > 5410 Then Return "MAXIMUM WIDTH IS 5410MM !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Panel Only" Then
            If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
            If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
                If drop < 600 Then Return "MINIMUM DROP IS 600MM !"
                If drop > 3200 Then Return "MAXIMUM DROP IS 3200MM !"
            End If
        End If

        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.wandlength) Then Return "WAND LENGTH IS REQUIRED !"
        End If

        If blindName = "Track Only" AndAlso Not data.wandlength = "Custom" Then Return "THE WAND LENGTH IS REQUIRED. PLEASE SELECT <b>CUSTOM</b> AND ENTER THE WAND LENGTH VALUE. !"

        If blindName = "Complete Set" AndAlso data.wandlength = "Custom" Then
            If String.IsNullOrEmpty(data.wandlengthvalue) Then Return "WAND LENGTH VALUE IS REQUIRED !"
            If Not Integer.TryParse(data.wandlengthvalue, wandlength) OrElse wandlength <= 0 Then Return "WAND LENGTH VALUE IS REQUIRED !"

            Dim thisStandard As Integer = Math.Ceiling(drop * 2 / 3)
            If thisStandard > 1000 Then thisStandard = 1000
            If wandlength < thisStandard Then Return String.Format("MINIMUM WAND LENGTH IS {0}MM !", thisStandard)
        End If

        If blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.wandlengthvalue) Then Return "WAND LENGTH VALUE IS REQUIRED !"
            If Not Integer.TryParse(data.wandlengthvalue, wandlength) OrElse wandlength <= 0 Then Return "WAND LENGTH VALUE IS REQUIRED !"
            If wandlength > 1000 Then Return "MAXIMUM WAND LENGTH IS 1000MM !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.tracktype) Then Return "TRACK TYPE IS REQUIRED !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.layoutcode) Then Return "LAYOUT CODE IS REQUIRED !"
            If data.layoutcode = "S" AndAlso String.IsNullOrEmpty(data.layoutcodecustom) Then
                Return "CUSTOM LAYOUT CODE IS REQUIRED !"
            End If
        End If

        If Not String.IsNullOrEmpty(data.panelqty) Then
            If Not Integer.TryParse(data.panelqty, panelQty) OrElse panelQty < 0 Then Return "PLEASE CHECK YOUT PANEL QTY ORDER !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Panel Only" Then
            If tubeName = "Plantation" Then
                If String.IsNullOrEmpty(data.batten) Then Return "FRONT BATTEN COLOUR IS REQUIRED !"
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If blindName = "Complete Set" Then
            If data.wandlength = "Standard" Then
                wandlength = Math.Ceiling(drop * 2 / 3)
                If wandlength > 1000 Then wandlength = 1000
            End If
            If Not data.layoutcode = "S" Then data.layoutcodecustom = String.Empty

            wandcolour = orderClass.GetItemData("SELECT ProductColours.Name FROM ProductColours LEFT JOIN Products ON ProductColours.Id=Products.ColourType WHERE Products.Id='" & data.colourtype & "'")

            linearMetre = width / 1000
            squareMetre = width * drop / 1000000
        End If

        If blindName = "Track Only" Then
            drop = 0
            data.fabrictype = String.Empty : data.fabriccolour = String.Empty

            wandcolour = orderClass.GetItemData("SELECT ProductColours.Name FROM ProductColours LEFT JOIN Products ON ProductColours.Id=Products.ColourType WHERE Products.Id='" & data.colourtype & "'")

            linearMetre = width / 1000
            squareMetre = 0
        End If

        If blindName = "Panel Only" Then
            data.mounting = String.Empty
            data.wandlength = String.Empty : wandlength = 0
            data.tracktype = String.Empty
            data.layoutcode = String.Empty
            data.layoutcodecustom = String.Empty

            linearMetre = width / 1000
            squareMetre = width * drop / 1000000
        End If

        If tubeName = "Plain" OrElse tubeName = "Sewless" Then
            data.batten = String.Empty
        End If

        ' FOR PRICING
        Dim groupFabric As String = orderClass.GetFabricGroup(data.fabrictype)
        If data.companyid = "3" Then
            groupFabric = orderClass.GetFabricGroupLocal("Panel", data.fabrictype)
        End If
        Dim groupName As String = String.Format("{0} - {1} - {2} - {3}", designName, blindName, tubeName, groupFabric)
        If blindName = "Track Only" Then groupName = String.Format("{0} - {1}", designName, blindName)

        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], WandColour, WandLength, WandLengthValue, LayoutCode, LayoutCodeCustom, TrackType, PanelQty, Batten, LinearMetre, SquareMetre, TotalItems, Notes, Markup, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @WandColour, @WandLength, @WandLengthValue, @LayoutCode, @LayoutCodeCustom, @TrackType, @PanelQty, @Batten, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@WandColour", wandcolour)
                        myCmd.Parameters.AddWithValue("@WandLength", data.wandlength)
                        myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                        myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@LayoutCodeCustom", data.layoutcodecustom)
                        myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                        myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                        myCmd.Parameters.AddWithValue("@Batten", data.batten)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" Or data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, WandColour=@WandColour, WandLength=@WandLength, WandLengthValue=@WandLengthValue, LayoutCode=@LayoutCode, LayoutCodeCustom=@LayoutCodeCustom, TrackType=@TrackType, PanelQty=@PanelQty, Batten=@Batten, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, TotalItems=1, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", 1)
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@WandColour", wandcolour)
                    myCmd.Parameters.AddWithValue("@WandLength", data.wandlength)
                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                    myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                    myCmd.Parameters.AddWithValue("@LayoutCodeCustom", data.layoutcodecustom)
                    myCmd.Parameters.AddWithValue("@TrackType", data.tracktype)
                    myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                    myCmd.Parameters.AddWithValue("@Batten", data.batten)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function PelmetProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer
        Dim widthb As Integer
        Dim widthc As Integer
        Dim rlvalue As Integer
        Dim rlvalueb As Integer
        Dim markup As Integer

        Dim linearMetre As Decimal
        Dim linearMetreB As Decimal
        Dim linearMetreC As Decimal

        Dim totalItems As Integer = 1

        Dim designName As String = String.Empty
        Dim tubeName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)
        Dim roleName As String = orderClass.GetUserRoleName(data.loginid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "BLIND TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.tubetype) Then Return "PELMET TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PLEASE CONTACT REZA@BIGBLINDS.CO.ID"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.layoutcode) Then Return "PELMET LAYOUT IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then

        End If
        If width > 9999 Then Return "MAXIMUM WIDTH IS 9999MM !"

        If data.layoutcode = "B" OrElse data.layoutcode = "C" Then
            If String.IsNullOrEmpty(data.widthb) Then Return "SECOND WIDTH IS REQUIRED !"
            If Not Integer.TryParse(data.widthb, widthb) OrElse widthb <= 0 Then Return "PLEASE CHECK YOUR SECOND WIDTH ORDER !"
            If width + widthb > 9999 Then Return "MAXIMUM TOTAL WIDTH IS 9999MM !"
        End If
        If data.layoutcode = "D" Then
            If String.IsNullOrEmpty(data.widthc) Then Return "THIRD WIDTH IS REQUIRED !"
            If Not Integer.TryParse(data.widthc, widthc) OrElse widthc <= 0 Then Return "PLEASE CHECK YOUR THIRD WIDTH ORDER !"
            If width + widthb + widthc > 9999 Then Return "MAXIMUM TOTAL WIDTH IS 9999MM !"
        End If
        If String.IsNullOrEmpty(data.returnposition) Then Return "RETURN POSITION IS REQUIRED !"

        If data.returnposition = "Left" OrElse data.returnposition = "Both Sides" Then
            If String.IsNullOrEmpty(data.returnlengthvalue) Then Return "RETURN LENGTH (LEFT) IS REQUIRED !"
            If Not Integer.TryParse(data.returnlengthvalue, rlvalue) OrElse rlvalue <= 0 Then Return "PLEASE CHECK YOUR RETURN LENGTH (LEFT) ORDER !"
        End If

        If data.returnposition = "Right" OrElse data.returnposition = "Both Sides" Then
            If String.IsNullOrEmpty(data.returnlengthvalueb) Then Return "RETURN LENGTH (RIGHT) IS REQUIRED !"
            If Not Integer.TryParse(data.returnlengthvalueb, rlvalueb) OrElse rlvalueb <= 0 Then Return "PLEASE CHECK YOUR RETURN LENGTH (RIGHT) ORDER !"
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        linearMetre = width / 1000

        If data.layoutcode = "A" Then
            widthb = 0 : widthc = 0
        End If

        If data.layoutcode = "B" OrElse data.layoutcode = "C" Then
            widthc = 0
            linearMetreB = widthb / 1000
            totalItems = 2
        End If

        If data.layoutcode = "D" Then
            linearMetreC = widthc / 1000
            totalItems = 3
        End If

        If data.returnposition = "Left" Then rlvalueb = 0
        If data.returnposition = "Right" Then rlvalue = 0

        Dim sellName As String = designName
        Dim groupName As String = String.Format("{0}", tubeName)
        If Not String.IsNullOrEmpty(data.batten) Then
            groupName = String.Format("{0} - Batten", tubeName)
        End If
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)
        Dim priceProductGroupB As String = String.Empty
        Dim priceProductGroupC As String = String.Empty

        If data.layoutcode = "B" OrElse data.layoutcode = "C" Then
            priceProductGroupB = priceProductGroup
        End If

        If data.layoutcode = "D" Then
            priceProductGroupB = priceProductGroup
            priceProductGroupC = priceProductGroup
        End If

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, PriceProductGroupId, PriceProductGroupIdB, PriceProductGroupIdC, Qty, Room, Mounting, Width, WidthB, WidthC, [Drop], DropB, DropC, LayoutCode, Batten, ReturnPosition, ReturnLengthValue, ReturnLengthValueB, LinearMetre, LinearMetreB, LinearMetreC, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @PriceProductGroupId, @PriceProductGroupIdB, @PriceProductGroupIdC, @Qty, @Room, @Mounting, @Width, @WidthB, @WidthC, 0, 0, 0, @LayoutCode, @Batten, @ReturnPosition, @ReturnLengthValue, @ReturnLengthValueB, @LinearMetre, @LinearMetreB, @LinearMetreC, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdC", If(String.IsNullOrEmpty(priceProductGroupC), CType(DBNull.Value, Object), priceProductGroupC))
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@WidthB", widthb)
                        myCmd.Parameters.AddWithValue("@WidthC", widthc)
                        myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@ReturnPosition", data.returnposition)
                        myCmd.Parameters.AddWithValue("@ReturnLengthValue", rlvalue)
                        myCmd.Parameters.AddWithValue("@ReturnLengthValueB", rlvalueb)
                        myCmd.Parameters.AddWithValue("@Batten", data.batten)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                        myCmd.Parameters.AddWithValue("@LinearMetreC", linearMetreC)
                        myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, PriceProductGroupId=@PriceProductGroupId, PriceProductGroupIdB=@PriceProductGroupIdB, PriceProductGroupIdC=@PriceProductGroupIdC, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, WidthB=@WidthB, WidthC=@WidthC, [Drop]=0, DropB=0, DropC=0, LayoutCode=@LayoutCode, Batten=@Batten, ReturnPosition=@ReturnPosition, ReturnLengthValue=@ReturnLengthValue, ReturnLengthValueB=@ReturnLengthValueB, LinearMetre=@LinearMetre, LinearMetreB=@LinearMetreB, LinearMetreC=@LinearMetreC, TotalItems=@TotalItems, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdC", If(String.IsNullOrEmpty(priceProductGroupC), CType(DBNull.Value, Object), priceProductGroupC))
                    myCmd.Parameters.AddWithValue("@Qty", 1)
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@WidthB", widthb)
                    myCmd.Parameters.AddWithValue("@WidthC", widthc)
                    myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                    myCmd.Parameters.AddWithValue("@ReturnPosition", data.returnposition)
                    myCmd.Parameters.AddWithValue("@ReturnLengthValue", rlvalue)
                    myCmd.Parameters.AddWithValue("@ReturnLengthValueB", rlvalueb)
                    myCmd.Parameters.AddWithValue("@Batten", data.batten)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                    myCmd.Parameters.AddWithValue("@LinearMetreC", linearMetreC)
                    myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function RomanProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer : Dim drop As Integer
        Dim controllength As Integer
        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim tubeName As String = String.Empty
        Dim controlName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)
        If Not String.IsNullOrEmpty(data.controltype) Then controlName = orderClass.GetControlName(data.controltype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "BLIND TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.tubetype) Then Return "ROMAN STYLE IS REQUIRED !"
        If String.IsNullOrEmpty(data.controltype) Then Return "CONTROL TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PLEASE CONTACT REZA@BIGBLINDS.CO.ID"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If controlName = "Chain" OrElse controlName = "Reg Cord Lock" OrElse controlName = "Cord Lock" Then
            If width < 360 Then Return "MINIMUM WIDTH IS 360MM !"
        End If
        If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" Then
            If width < 700 Then Return "MINIMUM WIDTH FOR MOTORISED IS 700MM !"
        End If

        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If width > 2910 Then Return "MAXIMUM WIDTH IS 2910MM !"
        End If

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
        If drop < 600 Then Return "MINIMUM DROP IS 600MM !"
        If data.companyid = "2" AndAlso (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If drop > 3200 Then Return "MAXIMUM DROP IS 3200MM !"
        End If

        If String.IsNullOrEmpty(data.valanceoption) Then Return "VALANCE OPTION IS REQUIRED !"
        If String.IsNullOrEmpty(data.controlposition) Then Return "CONTROL POSITION IS REQUIRED !"
        If String.IsNullOrEmpty(data.chaincolour) AndAlso (controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Chain") Then
            If controlName = "Chain" Then Return "CHAIN COLOUR IS REQUIRED"
            Return "REMOTE TYPE IS REQUIRED !"
        End If
        If controlName.Contains("Cord Lock") AndAlso String.IsNullOrEmpty(data.controlcolour) Then Return "CORD COLOUR IS REQUIRED !"

        If controlName = "Chain" OrElse controlName = "Reg Cord Lock" Then
            If String.IsNullOrEmpty(data.controllength) Then
                If controlName = "Chain" Then Return "CHAIN LENGTH IS REQUIRED"
                If controlName.Contains("Cord Lock") Then Return "CORD LENGTH IS REQUIRED !"
            End If

            If data.controllength = "Custom" Then
                If controlName = "Chain" Then
                    If String.IsNullOrEmpty(data.chainlengthvalue) Then Return "CHAIN LENGTH VALUE IS REQUIRED !"
                    If Not Integer.TryParse(data.chainlengthvalue, controllength) OrElse controllength <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE ORDER !"
                End If
                If controlName = "Reg Cord Lock" Then
                    If String.IsNullOrEmpty(data.cordlengthvalue) Then Return "CORD LENGTH VALUE IS REQUIRED !"
                    If Not Integer.TryParse(data.cordlengthvalue, controllength) OrElse controllength <= 0 Then Return "PLEASE CHECK YOUR CORD LENGTH VALUE ORDER !"
                End If
            End If
        End If

        If tubeName = "Plantation" AndAlso String.IsNullOrEmpty(data.batten) Then Return "BATTEN COLOUR IS REQUIRED !"

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If tubeName = "Classic" OrElse tubeName = "Sewless" Then data.batten = String.Empty

        If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Chain" Then
            data.controlcolour = String.Empty
        End If
        If controlName = "Reg Cord Lock" OrElse controlName = "Cord Lock" Then
            data.chaincolour = String.Empty
        End If

        If controlName = "Chain" OrElse controlName = "Reg Cord Lock" OrElse controlName = "Cord Lock" OrElse controlName = "Altus" OrElse controlName = "Mercure" Then
            data.charger = String.Empty
        End If
        If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" Then
            data.controllength = String.Empty
        End If
        If controlName = "Chain" OrElse controlName = "Reg Cord Lock" OrElse controlName = "Cord Lock" OrElse controlName = "Mercure" OrElse controlName = "Altus" OrElse controlName = "Sonesse 30 WF" Then
            data.extensioncable = String.Empty : data.supply = String.Empty
        End If

        If data.companyid = "2" Then data.drycontact = String.Empty
        If controlName = "Mercure" OrElse controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Chain" Then
            data.drycontact = String.Empty
        End If

        Dim linearMetre As Decimal = width / 1000
        Dim squareMetre As Decimal = width * drop / 1000000

        Dim groupFabric As String = orderClass.GetFabricGroup(data.fabrictype)
        If data.companyid = "3" Then
            groupFabric = orderClass.GetFabricGroupLocal("Roman", data.fabrictype)
        End If

        Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeName, groupFabric)
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)

        If data.controllength = "Standard" Then
            If controlName = "Reg Cord Lock" OrElse controlName = "Cord Lock" Then controllength = Math.Ceiling(drop * 2 / 3)
            If controlName = "Chain" Then
                controllength = 550
                Dim thisFormula As Integer = Math.Ceiling(drop * 2 / 3)
                If thisFormula > 500 Then controllength = 750
                If thisFormula > 750 Then controllength = 1000
                If thisFormula > 1000 Then controllength = 1200
                If thisFormula > 1200 Then controllength = 1500
            End If
        End If

        If width > 1510 OrElse drop > 1510 Then
            orderClass.DeleteFilePrinting(data.orderid, data.printing)
            data.printing = String.Empty
        End If

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], ControlPosition, ControlColour, ControlLength, ControlLengthValue, ValanceOption, Batten, DryContact, Charger, ExtensionCable, Supply, Printing, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @ControlPosition, @ControlColour, @ControlLength, @ControlLengthValue, @ValanceOption, @Batten, @DryContact, @Charger, @ExtensionCable, @Supply, @Printing, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@FabricId", data.fabrictype)
                        myCmd.Parameters.AddWithValue("@FabricColourId", data.fabriccolour)
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ControlColour", data.controlcolour)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                        myCmd.Parameters.AddWithValue("@ValanceOption", data.valanceoption)
                        myCmd.Parameters.AddWithValue("@Batten", data.batten)
                        myCmd.Parameters.AddWithValue("@DryContact", data.drycontact)
                        myCmd.Parameters.AddWithValue("@Charger", data.charger)
                        myCmd.Parameters.AddWithValue("@ExtensionCable", data.extensioncable)
                        myCmd.Parameters.AddWithValue("@Supply", data.supply)
                        myCmd.Parameters.AddWithValue("@Printing", data.printing)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, ChainId=@ChainId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, ControlPosition=@ControlPosition, ControlColour=@ControlColour, ControlLength=@ControlLength, ControlLengthValue=@ControlLengthValue, ValanceOption=@ValanceOption, Batten=@Batten, DryContact=@DryContact, Charger=@Charger, ExtensionCable=@ExtensionCable, Supply=@Supply, Printing=@Printing, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, TotalItems=1, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@FabricId", data.fabrictype)
                    myCmd.Parameters.AddWithValue("@FabricColourId", data.fabriccolour)
                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@ControlColour", data.controlcolour)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                    myCmd.Parameters.AddWithValue("@ValanceOption", data.valanceoption)
                    myCmd.Parameters.AddWithValue("@Batten", data.batten)
                    myCmd.Parameters.AddWithValue("@DryContact", data.drycontact)
                    myCmd.Parameters.AddWithValue("@Charger", data.charger)
                    myCmd.Parameters.AddWithValue("@ExtensionCable", data.extensioncable)
                    myCmd.Parameters.AddWithValue("@Supply", data.supply)
                    myCmd.Parameters.AddWithValue("@Printing", data.printing)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function PrivacyProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer
        Dim drop As Integer
        Dim controllength As Integer
        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "PRIVACY TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PRIVACY COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.controlposition) Then Return "CONTROL POSITION IS REQUIRED !"
        If String.IsNullOrEmpty(data.tilterposition) Then Return "TILTER POSITION IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If width < 450 Then Return "MINIMUM WIDTH IS 450MM !"
        If data.companyid = "2" OrElse (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If width > 2400 Then Return "MAXIMUM WIDTH IS 2400MM !"
        End If

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
        If drop < 200 Then Return "MINIMUM DROP IS 200MM !"
        If data.companyid = "2" OrElse (data.rolename = "Customer" OrElse data.rolename = "Installer") Then
            If drop > 2400 Then Return "MAXIMUM DROP IS 2400MM !"
        End If

        If String.IsNullOrEmpty(data.controllength) Then Return "CORD LENGTH IS REQUIRED !"

        If data.controllength = "Custom" Then
            If String.IsNullOrEmpty(data.controllengthvalue) Then Return "CORD LENGTH VALUE IS REQUIRED !"
            If Not Integer.TryParse(data.controllengthvalue, controllength) OrElse controllength < 0 Then
                Return "PLEASE CHECK YOUR CORD LENGTH ORDER !"
            End If

            Dim thisStandard As Integer = Math.Ceiling(drop * 2 / 3)
            If thisStandard < 550 Then thisStandard = 550
            If controllength < thisStandard Then Return String.Format("MINIMUM CORD LENGTH IS {0}MM !", thisStandard)
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If data.controllength = "Standard" Then
            controllength = Math.Ceiling(drop * 2 / 3)
            If controllength < 550 Then controllength = 550
        End If

        Dim linearMetre As Decimal = width / 1000
        Dim squareMetre As Decimal = width * drop / 1000000

        Dim groupName As String = blindName
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, Qty, Room, Mounting, ControlPosition, TilterPosition, Width, [Drop], Supply, ControlLength, ControlLengthValue, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, 1, @Room, @Mounting, @ControlPosition, @TilterPosition, @Width, @Drop, @Supply, @ControlLength, @ControlLengthValue, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@TilterPosition", data.tilterposition)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                        myCmd.Parameters.AddWithValue("@Supply", data.supply)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, PriceProductGroupId=@PriceProductGroupId, Qty=1, Room=@Room, Mounting=@Mounting, ControlPosition=@ControlPosition, TilterPosition=@TilterPosition, Width=@Width, [Drop]=@Drop, Supply=@Supply, ControlLength=@ControlLength, ControlLengthValue=@ControlLengthValue, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, TotalItems=1, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@TilterPosition", data.tilterposition)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                    myCmd.Parameters.AddWithValue("@Supply", data.supply)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function VenetianProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer : Dim widthb As Integer
        Dim drop As Integer : Dim dropb As Integer

        Dim clvalue As Integer : Dim clvalueb As Integer

        Dim vsvalue As Integer
        Dim rlvalue As Integer

        Dim markup As Integer

        Dim controlpositionb As String = String.Empty
        Dim tilterpositionb As String = String.Empty

        Dim linearMetre As Decimal : Dim linearMetreB As Decimal : Dim linearMetreC As Decimal

        Dim squareMetre As Decimal : Dim squareMetreB As Decimal : Dim squareMetreC As Decimal

        Dim totalItems As Integer = 1

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "VENETIAN TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "VENETIAN COLOUR IS REQUIRED !"
        If String.IsNullOrEmpty(data.subtype) Then Return "SUB VENETIAN TYPE IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" OrElse blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" Then
            If String.IsNullOrEmpty(data.tassel) Then Return "TASSEL OPTION IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If data.subtype = "Single" AndAlso (blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" OrElse blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" OrElse blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm") Then
            If width < 250 Then Return "MINIMUM WIDTH IS 250MM !"
        End If
        If data.subtype.Contains("2 on 1") Then
            If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" Then
                If width < 300 Then Return "MINIMUM WIDTH IS 300MM !"
            End If
            If blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" OrElse blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm" Then
                If width < 300 Then Return "MINIMUM WIDTH IS 300MM !"
            End If
        End If
        If blindName = "Econo 50mm (Cordless)" OrElse blindName = "Ultraslat 50mm (Cordless)" Then
            If width < 610 Then Return "MINIMUM WIDTH IS 610MM !"
        End If
        If blindName = "Econo 50mm (Cordless)" OrElse blindName = "Ultraslat 50mm (Cordless)" Then
            If width > 2410 Then Return "MINIMUM WIDTH IS 2410MM !"
        End If
        If width > 2710 Then Return "MAXIMUM WIDTH IS 2710MM !"

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
        If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" Then
            If drop < 200 Then Return "MINIMUM DROP IS 200MM !"
            If drop > 3200 Then Return "MINIMUM DROP IS 3200MM !"
        End If
        If blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" OrElse blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm" Then
            If drop < 200 Then Return "MINIMUM DROP IS 200MM !"
            If drop > 3200 Then Return "MINIMUM DROP IS 3200MM !"
        End If
        If blindName = "Econo 50mm (Cordless)" OrElse blindName = "Ultraslat 50mm (Cordless)" Then
            If drop < 600 Then Return "MINIMUM DROP IS 600MM !"
            If drop > 2410 Then Return "MINIMUM DROP IS 2500MM !"
        End If

        If data.subtype = "Single" AndAlso (blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" OrElse blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" OrElse blindName = "Ultraslat 50mm" OrElse blindName = "Ultraslat 63mm") Then
            If String.IsNullOrEmpty(data.controlposition) Then Return "CONTROL POSITION IS REQUIRED !"
        End If
        If data.subtype = "Single" AndAlso String.IsNullOrEmpty(data.tilterposition) Then Return "TILTER POSITION IS REQUIRED !"

        If width > 300 AndAlso width <= 400 AndAlso data.controlposition = data.tilterposition Then
            Return "PLEASE USE OPPOSITE CONTROL AND TILTER POSITIONS!"
        End If

        If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" OrElse blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" Then
            If String.IsNullOrEmpty(data.controllength) Then Return "CORD LENGTH IS REQUIRED !"

            If data.controllength = "Custom" Then
                If String.IsNullOrEmpty(data.controllengthvalue) Then Return "CONTROL LENGTH VALUE IS REQUIRED !"
                If Not Integer.TryParse(data.controllengthvalue, clvalue) OrElse clvalue < 0 Then Return "PLEASE CHECK YOUR CORD LENGTH ORDER !"

                Dim thisStandard As Integer = Math.Ceiling(drop * 2 / 3)
                If thisStandard < 550 Then thisStandard = 550
                If clvalue < thisStandard Then Return String.Format("MINIMUM CORD LENGTH IS {0}MM !", thisStandard)
            End If
        End If

        If blindName = "Econo 50mm (Cordless)" AndAlso String.IsNullOrEmpty(data.wandlengthvalue) Then Return "WAND LENGTH IS REQUIRED !"

        If data.subtype.Contains("2 on 1") Then
            If String.IsNullOrEmpty(data.widthb) Then Return "SECOND WIDTH IS REQUIRED !"
            If Not Integer.TryParse(data.widthb, widthb) OrElse widthb <= 0 Then Return "PLEASE CHECK YOUR SECOND WIDTH ORDER !"

            If widthb < 300 Then Return "MINIMUM WIDTH FOR SECOND BLIND IS 300MM !"
            If widthb > 2710 Then Return "MAXIMUM WIDTH FOR SECOND BLIND IS 2710MM !"
            If width + widthb >= 4000 Then Return "MAXIMUM TOTAL WIDTH IS 4000MM !"

            If String.IsNullOrEmpty(data.dropb) Then Return "SECOND DROP IS REQUIRED !"
            If Not Integer.TryParse(data.dropb, dropb) OrElse dropb <= 0 Then Return "PLEASE CHECK YOUR SECOND DROP ORDER !"

            If String.IsNullOrEmpty(data.controllengthb) Then Return "SECOND CORD LENGTH IS REQUIRED !"
            If data.controllengthb = "Custom" Then
                If String.IsNullOrEmpty(data.controllengthvalueb) Then Return "SECOND CORD LENGTH VALUE IS REQUIRED !"
                If Not String.IsNullOrEmpty(data.controllengthvalueb) Then
                    If Not Integer.TryParse(data.controllengthvalueb, clvalueb) OrElse clvalueb < 0 Then Return "PLEASE CHECK YOUR SECOND CORD LENGTH ORDER !"

                    Dim thisStandard As Integer = Math.Ceiling(dropb * 2 / 3)
                    If thisStandard < 550 Then thisStandard = 550
                    If clvalueb < thisStandard Then Return String.Format("MINIMUM SECOND CORD LENGTH IS {0}MM !", thisStandard)
                End If
            End If
        End If

        If String.IsNullOrEmpty(data.valancetype) Then Return "VALANCE TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.valancesize) Then Return "VALANCE SIZE IS REQUIRED !"
        If data.valancesize = "Custom" Then
            If String.IsNullOrEmpty(data.valancesizevalue) Then Return "VALANCE SIZE VALUE IS REQUIRED !"
            If Not Integer.TryParse(data.valancesizevalue, vsvalue) OrElse vsvalue < 0 Then Return "PLEASE CHECK YOUR VALANCE SIZE VALUE ORDER !"
        End If

        If Not String.IsNullOrEmpty(data.returnposition) Then
            If String.IsNullOrEmpty(data.returnlength) Then Return "VALANCE RETURN LENGTH IS REQUIRED !"
            If data.returnlength = "Custom" Then
                If String.IsNullOrEmpty(data.returnlengthvalue) Then Return "VALANCE RETURN LENGTH VALUE IS REQUIRED !"
                If Not Integer.TryParse(data.returnlengthvalue, rlvalue) OrElse rlvalue < 0 Then Return "PLEASE CHECK YOUR VALANCE RETURN LENGTH VALUE ORDER !"
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        linearMetre = width / 1000
        squareMetre = width * drop / 1000000

        If blindName = "Econo 50mm (Cordless)" Then
            widthb = 0 : dropb = 0
            data.controlposition = String.Empty
            data.controllength = String.Empty : clvalue = 0
            data.controllengthb = String.Empty : clvalueb = 0
            data.tassel = String.Empty
            clvalue = data.wandlengthvalue
        End If

        If blindName = "Basswood 50mm" OrElse blindName = "Basswood 63mm" OrElse blindName = "Econo 50mm" OrElse blindName = "Econo 63mm" Then
            If data.subtype = "Single" Then
                widthb = 0 : dropb = 0
                data.controllengthb = String.Empty
                clvalueb = 0
                widthb = 0 : dropb = 0
            End If

            If data.subtype = "2 on 1 Left-Left" Then
                data.controlposition = "Left"
                data.tilterposition = "Left"
                controlpositionb = "Left"
                tilterpositionb = String.Empty

                linearMetreB = widthb / 1000
                squareMetreB = widthb * dropb / 1000000

                totalItems = 2
            End If

            If data.subtype = "2 on 1 Right-Right" Then
                data.controlposition = "Right"
                data.tilterposition = String.Empty
                controlpositionb = "Right"
                tilterpositionb = "Right"

                linearMetreB = widthb / 1000
                squareMetreB = widthb * dropb / 1000000

                totalItems = 2
            End If

            If data.subtype = "2 on 1 Left-Right" Then
                data.controlposition = "Left"
                data.tilterposition = "Right"
                controlpositionb = "Left"
                tilterpositionb = "Right"

                linearMetreB = widthb / 1000
                squareMetreB = widthb * dropb / 1000000

                totalItems = 2
            End If

            If data.controllength = "Standard" Then
                clvalue = Math.Ceiling(drop * 2 / 3)
                If clvalue < 550 Then clvalue = 550
            End If

            If data.controllengthb = "Standard" Then
                clvalueb = Math.Ceiling(dropb * 2 / 3)
                If clvalueb < 550 Then clvalueb = 550
            End If

            If String.IsNullOrEmpty(data.returnposition) Then
                data.returnlength = String.Empty
                rlvalue = 0
            End If
        End If

        If data.valancesize = "Standard" Then
            vsvalue = width + widthb - 1
            If data.mounting = "Make Size Reveal Fit" Then
                vsvalue = width + widthb + 9
            End If
            If data.mounting = "Make Size Face Fit" OrElse data.mounting = "Make Size Face Fit" Then
                vsvalue = width + widthb + 20
            End If
        End If

        If data.returnlength = "Standard" Then
            rlvalue = 70
            If blindName.Contains("Econo") Then rlvalue = 77
        End If

        Dim groupName As String = String.Format("{0} - {1}", designName, blindName)
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)
        Dim priceProductGroupB As String = String.Empty
        Dim priceProductGroupC As String = String.Empty

        If data.subtype.Contains("2 on 1") Then priceProductGroupB = priceProductGroup
        If data.subtype.Contains("3 on 1") Then priceProductGroupB = priceProductGroup : priceProductGroupC = priceProductGroup

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails (Id, HeaderId, ProductId, PriceProductGroupId, PriceProductGroupIdB, SubType, Qty, Room, Mounting, ControlPosition, ControlPositionB, TilterPosition, TilterPositionB, Width, WidthB, [Drop], DropB, Supply, Tassel, ControlLength, ControlLengthValue, ControlLengthB, ControlLengthValueB, WandLengthValue, ValanceType, ValanceSize, ValanceSizeValue, ReturnPosition, ReturnLength, ReturnLengthValue, LinearMetre, LinearMetreB, LinearMetreC, SquareMetre, SquareMetreB, SquareMetreC, TotalItems, Notes, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @PriceProductGroupIdB, @SubType, 1, @Room, @Mounting, @ControlPosition, @ControlPositionB, @TilterPosition, @TilterPositionB, @Width, @WidthB, @Drop, @DropB, @Supply, @Tassel, @ControlLength, @ControlLengthValue, @ControlLengthB, @ControlLengthValueB, @WandLengthValue, @ValanceType, @ValanceSize, @ValanceSizeValue, @ReturnPosition, @ReturnLength, @ReturnLengthValue, @LinearMetre, @LinearMetreB, @LinearMetreC, @SquareMetre, @SquareMetreB, @SquareMetreC, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@SubType", data.subtype)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@TilterPosition", data.tilterposition)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", clvalue)

                        myCmd.Parameters.AddWithValue("@ControlPositionB", controlpositionb)
                        myCmd.Parameters.AddWithValue("@TilterPositionB", tilterpositionb)
                        myCmd.Parameters.AddWithValue("@WidthB", widthb)
                        myCmd.Parameters.AddWithValue("@DropB", dropb)
                        myCmd.Parameters.AddWithValue("@ControlLengthB", data.controllengthb)
                        myCmd.Parameters.AddWithValue("@ControlLengthValueB", clvalueb)

                        myCmd.Parameters.AddWithValue("@WandLengthValue", data.wandlengthvalue)

                        myCmd.Parameters.AddWithValue("@ValanceType", data.valancetype)
                        myCmd.Parameters.AddWithValue("@ValanceSize", data.valancesize)
                        myCmd.Parameters.AddWithValue("@ValanceSizeValue", vsvalue)

                        myCmd.Parameters.AddWithValue("@ReturnPosition", data.returnposition)
                        myCmd.Parameters.AddWithValue("@ReturnLength", data.returnlength)
                        myCmd.Parameters.AddWithValue("@ReturnLengthValue", rlvalue)

                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                        myCmd.Parameters.AddWithValue("@LinearMetreC", linearMetreC)

                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                        myCmd.Parameters.AddWithValue("@SquareMetreC", squareMetreC)

                        myCmd.Parameters.AddWithValue("@Tassel", data.tassel)
                        myCmd.Parameters.AddWithValue("@Supply", data.supply)
                        myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, PriceProductGroupId=@PriceProductGroupId, PriceProductGroupIdB=@PriceProductGroupIdB, SubType=@Subtype, Qty=1, Room=@Room, Mounting=@Mounting, ControlPosition=@ControlPosition, ControlPositionB=@ControlPositionB, TilterPosition=@TilterPosition, TilterPositionB=@TilterPositionB, Width=@Width, WidthB=@WidthB, [Drop]=@Drop, DropB=@DropB, Supply=@Supply, Tassel=@Tassel, ControlLength=@ControlLength, ControlLengthB=@ControlLengthB, ControlLengthValue=@ControlLengthValue, ControlLengthValueB=@ControlLengthValueB, WandLengthValue=@WandLengthValue, ValanceType=@ValanceType, ValanceSize=@ValanceSize, ValanceSizeValue=@ValanceSizeValue, ReturnPosition=@ReturnPosition, ReturnLength=@ReturnLength, ReturnLengthValue=@ReturnLengthValue, LinearMetre=@LinearMetre, LinearMetreB=@LinearMetreB, LinearMetreC=@LinearMetreC, SquareMetre=@SquareMetre, SquareMetreB=@SquareMetreB, SquareMetreC=@SquareMetreC, TotalItems=@TotalItems, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@SubType", data.subtype)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@TilterPosition", data.tilterposition)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", clvalue)

                    myCmd.Parameters.AddWithValue("@ControlPositionB", controlpositionb)
                    myCmd.Parameters.AddWithValue("@TilterPositionB", tilterpositionb)
                    myCmd.Parameters.AddWithValue("@Widthb", widthb)
                    myCmd.Parameters.AddWithValue("@DropB", dropb)
                    myCmd.Parameters.AddWithValue("@ControlLengthB", data.controllengthb)
                    myCmd.Parameters.AddWithValue("@ControlLengthValueB", clvalueb)

                    myCmd.Parameters.AddWithValue("@WandLengthValue", data.wandlengthvalue)

                    myCmd.Parameters.AddWithValue("@ValanceType", data.valancetype)
                    myCmd.Parameters.AddWithValue("@ValanceSize", data.valancesize)
                    myCmd.Parameters.AddWithValue("@ValanceSizeValue", vsvalue)

                    myCmd.Parameters.AddWithValue("@ReturnPosition", data.returnposition)
                    myCmd.Parameters.AddWithValue("@ReturnLength", data.returnlength)
                    myCmd.Parameters.AddWithValue("@ReturnLengthValue", rlvalue)

                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                    myCmd.Parameters.AddWithValue("@LinearMetreC", linearMetreC)

                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                    myCmd.Parameters.AddWithValue("@SquareMetreC", squareMetreC)

                    myCmd.Parameters.AddWithValue("@Tassel", data.tassel)
                    myCmd.Parameters.AddWithValue("@Supply", data.supply)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function VerticalProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer : Dim qtyblade As Integer
        Dim width As Integer : Dim drop As Integer
        Dim controllength As Integer : Dim wandlength As Integer
        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty
        Dim tubeName As String = String.Empty
        Dim controlName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)
        If Not String.IsNullOrEmpty(data.controltype) Then controlName = orderClass.GetControlName(data.controltype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "VERTICAL SYSTEM IS REQUIRED !"
        If String.IsNullOrEmpty(data.tubetype) Then Return "SLAT TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.controltype) Then Return "CONTROL TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "TRACK COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"
        End If

        If blindName = "Track Only" AndAlso data.fabricinsert = "Yes" Then
            If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Slat Only" Then
            If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"
        End If

        If blindName = "Track Only" OrElse blindName = "Slat Only" Then
            If String.IsNullOrEmpty(data.qtyblade) Then Return "BLADE QTY IS REQUIRED !"
            If Not Integer.TryParse(data.qtyblade, qtyblade) OrElse qtyblade <= 0 Then Return "PLEASE CHECK YOUR BLADE QTY ORDER !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
            If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
            If width < 300 Then Return "MINIMUM WIDTH IS 300MM !"
            If tubeName = "89mm" AndAlso width > 5511 Then Return "MAXIMUM WIDTH IS 5511MM !"
            If tubeName = "127mm" AndAlso width > 5477 Then Return "MAXIMUM WIDTH IS 5477MM !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Slat Only" Then
            If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
            If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
            If drop < 300 Then Return "MINIMUM DROP IS 300MM !"
            If drop > 3050 Then Return "MAXIMUM DROP IS 3050MM !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.stackposition) Then Return "STACK POSITION IS REQUIRED !"
            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.controlposition) Then Return "CONTROL POSITION IS REQUIRED !"
                If String.IsNullOrEmpty(data.chaincolour) Then Return "CHAIN COLOUR IS REQUIRED !"
            End If
            If controlName = "Wand" Then
                If String.IsNullOrEmpty(data.wandcolour) Then Return "WAND COLOUR IS REQUIRED !"
            End If
        End If

        If blindName = "Complete Set" Then
            If String.IsNullOrEmpty(data.controllength) Then Return "CONTROL LENGTH IS REQUIRED !"
            If data.controllength = "Custom" Then
                If String.IsNullOrEmpty(data.controllengthvalue) Then Return "CONTROL LENGTH VALUE IS REQUIRED !"
                If Not Integer.TryParse(data.controllengthvalue, controllength) OrElse controllength < 0 Then Return "PLEASE CHECK YOUR CONTROL LENGTH ORDER !"

                Dim thisStandard As Integer = Math.Ceiling(drop * 2 / 3)
                If thisStandard > 1000 Then thisStandard = 1000
                If controllength < thisStandard Then
                    If thisStandard = 1000 Then Return "MINIMUM CONTROL LENGTH IS 1000MM !"
                    Return String.Format("CONTROL LENGTH MUST BE BETWEEN {0}MM - 1000MM !", thisStandard)
                End If
                If controllength > 1000 Then Return "MAXIMUM CONTROL LENGTH IS 1000MM !"
            End If
        End If

        If blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.controllengthvalue) Then Return "CONTROL LENGTH VALUE IS REQUIRED !"
            If Not Integer.TryParse(data.controllengthvalue, controllength) OrElse controllength < 0 Then Return "PLEASE CHECK YOUR CONTROL LENGTH ORDER !"
            If controllength > 1000 Then Return "MAXIMUM CONTROL LENGTH IS 1000MM !"
        End If

        If blindName = "Complete Set" OrElse blindName = "Slat Only" Then
            If String.IsNullOrEmpty(data.bottomjoining) Then Return "BOTTOM JOINING IS REQUIRED !"
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If blindName = "Complete Set" Then
            qtyblade = 0
            If data.controllength = "Standard" Then
                controllength = Math.Ceiling(drop * 2 / 3)
                If controllength > 1000 Then controllength = 1000
            End If
            If controlName = "Chain" Then
                data.wandcolour = String.Empty
            End If
            If controlName = "Wand" Then
                data.chaincolour = String.Empty
                wandlength = controllength

                If data.stackposition = "Left" Then data.controlposition = "Right"
                If data.stackposition = "Right" Then data.controlposition = "Left"
                If data.stackposition = "Centre" Then data.controlposition = "Right and Left"
                If data.stackposition = "Split" Then data.controlposition = "Middle"
            End If
        End If

        If blindName = "Slat Only" Then
            data.mounting = String.Empty
            data.fabricinsert = String.Empty
            data.stackposition = String.Empty
            data.controlposition = String.Empty
            data.controlcolour = String.Empty
            data.controllength = String.Empty
            controllength = 0 : wandlength = 0
            data.bracketextension = String.Empty
            data.sloping = String.Empty

            If tubeName = "127mm" Then
                width = qtyblade * 79 : If qtyblade < 6 Then width = 472
            End If
            If tubeName = "89mm" Then
                width = qtyblade * 115 : If qtyblade < 5 Then width = 591
            End If
        End If

        If blindName = "Track Only" Then
            If String.IsNullOrEmpty(data.fabricinsert) Then
                data.fabrictype = String.Empty : data.fabriccolour = String.Empty
            End If
            drop = 0
            data.bottomjoining = String.Empty
            data.controllength = "Custom"
            data.sloping = String.Empty

            If controlName = "Chain" Then
                data.wandcolour = String.Empty
            End If
            If controlName = "Wand" Then
                data.chaincolour = String.Empty
                wandlength = controllength

                If data.stackposition = "Stack Left" Then data.controlposition = "Right"
                If data.stackposition = "Stack Right" Then data.controlposition = "Left"
                If data.stackposition = "Stack Centre" Then data.controlposition = "Right and Left"
                If data.stackposition = "Stack Split" Then data.controlposition = "Middle"
            End If
        End If

        Dim linearMetre As Decimal = width / 1000
        Dim squareMetre As Decimal = width * drop / 1000000

        Dim groupName As String = String.Format("Vertical - {0} - {1}", blindName, tubeName)
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, QtyBlade, Room, Mounting, Width, [Drop], StackPosition, ControlPosition, ControlLength, ControlLengthValue, WandColour, WandLengthValue, FabricInsert, BottomJoining, BracketExtension, Sloping, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, @Qty, @QtyBlade, @Room, @Mounting, @Width, @Drop, @StackPosition, @ControlPosition, @ControlLength, @ControlLengthValue, @WandColour, @WandLengthValue, @FabricInsert, @BottomJoining, @BracketExtension, @Sloping, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@QtyBlade", qtyblade)
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@FabricInsert", data.fabricinsert)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                        myCmd.Parameters.AddWithValue("@BottomJoining", data.bottomjoining)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketextension)
                        myCmd.Parameters.AddWithValue("@Sloping", data.sloping)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, ChainId=@ChainId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, QtyBlade=@QtyBlade, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, StackPosition=@StackPosition, ControlPosition=@ControlPosition, ControlLength=@ControlLength, ControlLengthValue=@ControlLengthValue, WandColour=@WandColour, WandLengthValue=@WandLengthValue, FabricInsert=@FabricInsert, BottomJoining=@BottomJoining, BracketExtension=@BracketExtension, Sloping=@Sloping, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, TotalItems=1, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@QtyBlade", qtyblade)
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@FabricInsert", data.fabricinsert)
                    myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                    myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                    myCmd.Parameters.AddWithValue("@BottomJoining", data.bottomjoining)
                    myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketextension)
                    myCmd.Parameters.AddWithValue("@Sloping", data.sloping)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function RollerProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer
        Dim widthb As Integer
        Dim widthc As Integer
        Dim widthd As Integer
        Dim widthe As Integer
        Dim widthf As Integer
        Dim drop As Integer
        Dim dropb As Integer
        Dim dropc As Integer
        Dim dropd As Integer
        Dim drope As Integer
        Dim dropf As Integer

        Dim controllength As Integer
        Dim controllengthb As Integer
        Dim controllengthc As Integer
        Dim controllengthd As Integer
        Dim controllengthe As Integer
        Dim controllengthf As Integer

        Dim linearMetre As Decimal
        Dim linearMetreB As Decimal
        Dim linearMetreC As Decimal
        Dim linearMetreD As Decimal
        Dim linearMetreE As Decimal
        Dim linearMetreF As Decimal

        Dim squareMetre As Decimal
        Dim squareMetreB As Decimal
        Dim squareMetreC As Decimal
        Dim squareMetreD As Decimal
        Dim squareMetreE As Decimal
        Dim squareMetreF As Decimal

        Dim printing As String = String.Empty
        Dim printingb As String = String.Empty
        Dim printingc As String = String.Empty
        Dim printingd As String = String.Empty
        Dim printinge As String = String.Empty
        Dim printingf As String = String.Empty
        Dim printingg As String = String.Empty
        Dim printingh As String = String.Empty

        Dim totalItems As Integer = 1

        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty
        Dim tubeName As String = String.Empty
        Dim controlName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)
        If Not String.IsNullOrEmpty(data.controltype) Then controlName = orderClass.GetControlName(data.controltype)

        Dim chainType As String = String.Empty
        Dim chainTypeB As String = String.Empty
        Dim chainTypeC As String = String.Empty
        Dim chainTypeD As String = String.Empty
        Dim chainTypeE As String = String.Empty
        Dim chainTypeF As String = String.Empty

        Dim bottomName As String = String.Empty
        Dim bottomNameB As String = String.Empty
        Dim bottomNameC As String = String.Empty
        Dim bottomNameD As String = String.Empty
        Dim bottomNameE As String = String.Empty
        Dim bottomNameF As String = String.Empty

        If Not String.IsNullOrEmpty(data.chaincolour) Then chainType = orderClass.GetChainType(data.chaincolour)
        If Not String.IsNullOrEmpty(data.chaincolourb) Then chainTypeB = orderClass.GetChainType(data.chaincolourb)
        If Not String.IsNullOrEmpty(data.chaincolourc) Then chainTypeC = orderClass.GetChainType(data.chaincolourc)
        If Not String.IsNullOrEmpty(data.chaincolourd) Then chainTypeD = orderClass.GetChainType(data.chaincolourd)
        If Not String.IsNullOrEmpty(data.chaincoloure) Then chainTypeE = orderClass.GetChainType(data.chaincoloure)
        If Not String.IsNullOrEmpty(data.chaincolourf) Then chainTypeF = orderClass.GetChainType(data.chaincolourf)

        If Not String.IsNullOrEmpty(data.bottomtype) Then bottomName = orderClass.GetBottomName(data.bottomtype)
        If Not String.IsNullOrEmpty(data.bottomtypeb) Then bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
        If Not String.IsNullOrEmpty(data.bottomtypec) Then bottomNameC = orderClass.GetBottomName(data.bottomtypec)
        If Not String.IsNullOrEmpty(data.bottomtyped) Then bottomNameD = orderClass.GetBottomName(data.bottomtyped)
        If Not String.IsNullOrEmpty(data.bottomtypee) Then bottomNameE = orderClass.GetBottomName(data.bottomtypee)
        If Not String.IsNullOrEmpty(data.bottomtypef) Then bottomNameF = orderClass.GetBottomName(data.bottomtypef)

        If String.IsNullOrEmpty(data.blindtype) Then Return "ROLLER TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.tubetype) Then Return "TUBE TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.controltype) Then Return "CONTROL TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "BRACKET COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If

        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"
        If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
            If String.IsNullOrEmpty(data.remote) Then Return "REMOTE TYPE IS REQUIRED !"
        End If
        If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"
        If String.IsNullOrEmpty(data.roll) Then Return "ROLL DIRECTION IS REQUIRED !"
        If blindName = "Single Blind" OrElse blindName = "Wire Guide" OrElse blindName = "Full Cassette" OrElse blindName = "Semi Cassette" OrElse blindName = "Dual Blinds" OrElse blindName = "Link 2 Blinds Dependent" OrElse blindName = "Link 3 Blinds Dependent" OrElse blindName = "Link 3 Blinds Independent with Dependent" OrElse blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If String.IsNullOrEmpty(data.controlposition) Then
                If blindName = "Link 3 Blinds Independent with Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then Return "INDEPENDENT POSITION IS REQUIRED !"
                Return "CONTROL POSITION IS REQUIRED !"
            End If
        End If

        If controlName = "Chain" Then
            If String.IsNullOrEmpty(data.chaincolour) Then Return "CHAIN COLOUR IS REQUIRED !"
            If String.IsNullOrEmpty(data.chainstopper) Then Return "CHAIN STOPPER IS REQUIRED !"
            If String.IsNullOrEmpty(data.controllength) Then Return "CHAIN LENGTH IS REQUIRED !"

            If data.controllength = "Custom" Then
                If chainType = "Continuous" Then
                    If String.IsNullOrEmpty(data.controllengthvalue2) Then Return "CHAIN LENGTH VALUE IS REQUIRED !"
                    If Not Integer.TryParse(data.controllengthvalue2, controllength) OrElse controllength <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE ORDER !"
                End If
                If chainType = "Non Continuous" Then
                    If String.IsNullOrEmpty(data.controllengthvalue) Then Return "CHAIN LENGTH VALUE IS REQUIRED !"
                    If Not Integer.TryParse(data.controllengthvalue, controllength) OrElse controllength <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE ORDER !"
                End If
                Dim minimumControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                If tubeName.Contains("Gear Reduction") AndAlso chainType = "Non Continuous" Then
                    minimumControlLength = Math.Ceiling((drop * 3 / 4) + 80)
                End If
                If controllength < minimumControlLength Then
                    If chainType = "Continuous" Then
                        Return "MINIMUM CHAIN LENGTH IS 500MM !"
                        If minimumControlLength > 500 AndAlso minimumControlLength < 750 Then
                            Return "MINIMUM CHAIN LENGTH IS 750MM !"
                        End If
                        If minimumControlLength > 750 AndAlso minimumControlLength < 1000 Then
                            Return "MINIMUM CHAIN LENGTH IS 1000MM !"
                        End If
                        If minimumControlLength > 1000 AndAlso minimumControlLength < 1200 Then
                            Return "MINIMUM CHAIN LENGTH IS 1200MM !"
                        End If
                        If minimumControlLength > 1200 Then
                            Return "MINIMUM CHAIN LENGTH IS 1500MM !"
                        End If
                    End If
                    Return String.Format("MINIMUM CHAIN LENGTH IS {0}MM !", minimumControlLength)
                End If
            End If
        End If

        If Not blindName = "Full Cassette" AndAlso Not blindName = "Semi Cassette" Then
            If String.IsNullOrEmpty(data.bottomtype) Then Return "BOTTOM RAIL TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.bottomcolour) Then Return "BOTTOM RAIL COLOUR IS REQUIRED !"
            If bottomName = "Flat" Then
                If String.IsNullOrEmpty(data.bottomoption) Then Return "BOTTOM OPTION FOR BOTTOM FLAT IS REQUIRED !"
            End If
        End If

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        If width < 200 Then Return "MINIMUM WIDTH IS 200MM !"
        If data.companyid = "2" Then
            If width > 2400 AndAlso (blindName = "Full Cassette" OrElse blindName = "Semi Cassette") Then Return "MAXIMUM WIDTH IS 2400MM !"
            If width > 2910 Then Return "MAXIMUM WIDTH IS 2910MM !"
        End If

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"

        If drop < 600 Then Return "MINIUM DROP IS 600MM !"
        If data.companyid = "2" Then
            If width > 2200 AndAlso (blindName = "Full Cassette" OrElse blindName = "Semi Cassette") Then Return "MAXIMUM DROP IS 2200MM !"
            If drop > 3200 Then Return "MAXIMUM DROP IS 3200MM !"
        End If

        squareMetre = width * drop / 1000000

        If tubeName.Contains("Gear Reduction") Then
            If data.companyid = "2" Then
                If tubeName = "Gear Reduction 38mm" AndAlso width > 1810 Then Return "MAXIMUM WIDTH BLIND FOR GEAR REDUCTION 38MM IS 1810MM !"
                If squareMetre >= 6 AndAlso (tubeName = "Gear Reduction 38mm" OrElse tubeName = "Gear Reduction 45mm") Then
                    Return "YOUR BLIND AREA EXCEEDS 6 SQM.<br />PLEASE USE <b>GEAR REDUCTION 49MM</b> IF YOU WISH TO CONTINUE USING THE GEAR REDUCTION SYSTEM.<br />OUR ALTERNATIVE RECOMMENDATION:<br />ACMEDA SYSTEM: <b>ACMEDA 49MM</b><br />SUNBOSS SYSTEM: <b>SUNBOSS 43MM</b> OR <b>SUNBOSS 50MM</b>"
                End If
            End If
        End If

        ' START SECOND BLIND
        If blindName = "Dual Blinds" Then
            If String.IsNullOrEmpty(data.fabrictypeb) Then Return "FABRIC TYPE FOR SECOND BLIND IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolourb) Then Return "FABRIC COLOUR FOR SECOND BLIND IS REQUIRED !"
            If String.IsNullOrEmpty(data.rollb) Then Return "ROLL DIRECTION FOR SECOND BLIND IS REQUIRED !"
            If data.roll = "Standard" AndAlso data.rollb = "Standard" Then Return "STANDARD - STANDARD ROLL DIRECTION CANNOT BE USED !"
            If String.IsNullOrEmpty(data.controlpositionb) Then Return "CONTROL POSITION FOR SECOND BLIND IS REQUIRED !"
            If tubeName.Contains("Gear Reduction") Then
                If Not data.controlposition = data.controlpositionb Then
                    Return "PLEASE CHECK YOUR CONTROL POSITION. THE CONTROL POSITION FOR GEAR REDUCTION MUST BE THE SAME. !"
                End If
            End If
        End If

        If blindName = "Dual Blinds" OrElse blindName = "Link 2 Blinds Independent" OrElse blindName = "DB Link 2 Blinds Independent" Then
            If controlName = "Chain" Then
                If Not String.IsNullOrEmpty(data.chaincolourb) Then
                    If String.IsNullOrEmpty(data.chainstopperb) Then Return "CHAIN STOPPER FOR SECOND BLIND IS REQUIRED !"
                    If String.IsNullOrEmpty(data.controllengthb) Then Return "CHAIN LENGTH FOR SECOND BLIND IS REQUIRED !"
                    If data.controllengthb = "Custom" Then
                        chainTypeB = orderClass.GetChainType(data.chaincolourb)

                        If chainTypeB = "Continuous" Then
                            If String.IsNullOrEmpty(data.controllengthvalueb2) Then Return "CHAIN LENGTH VALUE FOR SECOND BLIND IS REQUIRED !"
                            If Not Integer.TryParse(data.controllengthvalueb2, controllengthb) OrElse controllengthb <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE FOR SECOND BLIND !"
                        End If
                        If chainTypeB = "Non Continuous" Then
                            If String.IsNullOrEmpty(data.controllengthvalueb) Then Return "CHAIN LENGTH VALUE FOR SECOND BLIND IS REQUIRED !"
                            If Not Integer.TryParse(data.controllengthvalueb, controllengthb) OrElse controllengthb <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE FOR SECOND BLIND !"
                        End If
                    End If
                End If
            End If
        End If

        If blindName = "Dual Blinds" OrElse blindName = "Link 2 Blinds Dependent" OrElse blindName = "Link 2 Blinds Independent" OrElse blindName = "Link 3 Blinds Dependent" OrElse blindName = "Link 3 Blinds Independent with Dependent" OrElse blindName = "Link 4 Blinds Independent with Dependent" OrElse blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" OrElse blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If Not String.IsNullOrEmpty(data.bottomtypeb) Then
                If String.IsNullOrEmpty(data.bottomcolourb) Then Return "BOTTOM COLOUR FOR SECOND BLIND IS REQUIRED !"
                If bottomNameB = "Flat" AndAlso String.IsNullOrEmpty(data.bottomoptionb) Then Return "FLAT BOTTOM FOR SECOND BLIND IS REQUIRED !"
            End If

            If String.IsNullOrEmpty(data.widthb) Then Return "WIDTH FOR SECOND BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.widthb, widthb) OrElse widthb <= 0 Then Return "PLEASE CHECK YOUR WIDTH FOR SECOND BLIND !"

            If widthb < 600 Then Return "MINIMUM SECOND WIDTH IS 600MM !"
            If widthb > 2910 Then Return "MAXIMUM SECOND WIDTH IS 2910MM !"

            If String.IsNullOrEmpty(data.dropb) Then Return "DROP FOR SECOND BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.dropb, dropb) OrElse dropb <= 0 Then Return "PLEASE CHECK YOUR DROP FOR SECOND BLIND !"

            If dropb < 500 Then Return "MINIMUM SECOND DROP IS 500MM !"
            If data.companyid = "2" Then
                If dropb > 3200 Then Return "MAXIMUM SECOND DROP IS 3200MM !"
            End If

            squareMetreB = widthb * dropb / 1000000

            If tubeName.Contains("Gear Reduction") Then
                If data.companyid = "2" Then
                    If tubeName = "Gear Reduction 38mm" AndAlso widthb > 1810 Then Return "MAXIMUM WIDTH FOR SECOND BLIND GEAR REDUCTION 38MM IS 1810MM !"
                    If squareMetreB >= 6 AndAlso (tubeName = "Gear Reduction 38mm" OrElse tubeName = "Gear Reduction 45mm") Then
                        Return "YOUR BLIND AREA EXCEEDS 6 SQM.<br />PLEASE USE <b>GEAR REDUCTION 49MM</b> IF YOU WISH TO CONTINUE USING THE GEAR REDUCTION SYSTEM.<br />OUR ALTERNATIVE RECOMMENDATION:<br />ACMEDA SYSTEM: <b>ACMEDA 49MM</b><br />SUNBOSS SYSTEM: <b>SUNBOSS 43MM</b> OR <b>SUNBOSS 50MM</b>"
                    End If
                End If
            End If
        End If
        ' END SECOND BLIND

        ' START THIRD BLIND
        If blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" Then
            If String.IsNullOrEmpty(data.fabrictypec) Then Return "FABRIC TYPE FOR THRID BLIND IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolourc) Then Return "FABRIC COLOUR FOR THRID BLIND IS REQUIRED !"
            If String.IsNullOrEmpty(data.rollc) Then Return "ROLL DIRECTION FOR THRID BLIND IS REQUIRED !"
        End If

        If blindName = "DB Link 2 Blinds Dependent" Then
            If String.IsNullOrEmpty(data.controlpositionc) Then Return "CONTROL POSITION FOR THRID BLIND IS REQUIRED !"
        End If

        If blindName = "Link 3 Blinds Independent with Dependent" OrElse blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" OrElse blindName = "DB Link 2 Blinds Independent with Dependent" Then
            If controlName = "Chain" Then
                If Not String.IsNullOrEmpty(data.chaincolourc) Then
                    If String.IsNullOrEmpty(data.chainstopperc) Then Return "CHAIN STOPPER FOR THIRD BLIND IS REQUIRED !"
                    If String.IsNullOrEmpty(data.controllengthc) Then Return "CHAIN LENGTH FOR THIRD BLIND IS REQUIRED !"
                    If data.controllengthc = "Custom" Then
                        chainTypeC = orderClass.GetChainType(data.chaincolourc)

                        If chainTypeC = "Continuous" Then
                            If String.IsNullOrEmpty(data.controllengthvaluec2) Then Return "CHAIN LENGTH VALUE FOR THIRD BLIND IS REQUIRED !"
                            If Not Integer.TryParse(data.controllengthvaluec2, controllengthc) OrElse controllengthc <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE FOR THIRD BLIND !"
                        End If
                        If chainTypeC = "Non Continuous" Then
                            If String.IsNullOrEmpty(data.controllengthvaluec) Then Return "CHAIN LENGTH VALUE FOR THIRD BLIND IS REQUIRED !"
                            If Not Integer.TryParse(data.controllengthvaluec, controllengthc) OrElse controllengthc <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE FOR THIRD BLIND !"
                        End If
                    End If
                End If
            End If
        End If

        If blindName = "Link 3 Blinds Dependent" OrElse blindName = "Link 3 Blinds Independent with Dependent" OrElse blindName = "Link 4 Blinds Independent with Dependent" OrElse blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" OrElse blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If Not String.IsNullOrEmpty(data.bottomtypec) Then
                If String.IsNullOrEmpty(data.bottomcolourc) Then Return "BOTTOM COLOUR FOR THIRD BLIND IS REQUIRED !"
                If bottomNameC = "Flat" AndAlso String.IsNullOrEmpty(data.bottomoptionc) Then Return "FLAT BOTTOM FOR THIRD BLIND IS REQUIRED !"
            End If

            If String.IsNullOrEmpty(data.widthc) Then Return "WIDTH FOR THIRD BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.widthc, widthc) OrElse widthc <= 0 Then Return "PLEASE CHECK YOUR WIDTH FOR THIRD BLIND !"

            If String.IsNullOrEmpty(data.dropc) Then Return "DROP FOR THIRD BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.dropc, dropc) OrElse dropc <= 0 Then Return "PLEASE CHECK YOUR DROP FOR THIRD BLIND !"

            squareMetreC = widthc * dropc / 1000000

            If tubeName.Contains("Gear Reduction") Then
                If data.companyid = "2" Then
                    If tubeName = "Gear Reduction 38mm" And widthc > 1810 Then Return "MAXIMUM WIDTH FOR SECOND BLIND GEAR REDUCTION 38MM IS 1810MM !"
                    If squareMetreC >= 6 AndAlso (tubeName = "Gear Reduction 38mm" OrElse tubeName = "Gear Reduction 45mm") Then
                        Return "YOUR BLIND AREA EXCEEDS 6 SQM.<br />PLEASE USE <b>GEAR REDUCTION 49MM</b> IF YOU WISH TO CONTINUE USING THE GEAR REDUCTION SYSTEM.<br />OUR ALTERNATIVE RECOMMENDATION:<br />ACMEDA SYSTEM: <b>ACMEDA 49MM</b><br />SUNBOSS SYSTEM: <b>SUNBOSS 43MM</b> OR <b>SUNBOSS 50MM</b>"
                    End If
                End If
            End If
        End If
        ' END THIRD BLIND

        ' START FOURTH BLIND
        If blindName = "DB Link 3 Blinds Dependent" Or blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If String.IsNullOrEmpty(data.fabrictyped) Then Return "FABRIC TYPE FOR SECOND ROLLER IS REQUIRED !"
            If String.IsNullOrEmpty(data.fabriccolourd) Then Return "FABRIC COLOUR FOR SECOND ROLLER IS REQUIRED !"
            If String.IsNullOrEmpty(data.rolld) Then Return "ROLL DIRECTION FOR SECOND ROLLER IS REQUIRED !"
        End If

        If blindName = "DB Link 3 Blinds Dependent" Then
            If String.IsNullOrEmpty(data.controlpositiond) Then Return "CONTROL POSITION FOR SECOND ROLLER IS REQUIRED !"
        End If

        If blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If Not String.IsNullOrEmpty(data.controlpositiond) Then
                If Not data.controlpositiond = data.controlposition Then
                    Return "POSISI INDEPENDENT HARUS SAMA ANTARA 2 SISI ROLLER !"
                End If
            End If
            If String.IsNullOrEmpty(data.controlpositiond) Then Return "CONTROL POSITION FOR SECOND ROLLER IS REQUIRED !"
        End If

        If blindName = "DB Link 2 Blinds Independent" OrElse blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If controlName = "Chain" AndAlso Not String.IsNullOrEmpty(data.chaincolourd) Then
                If String.IsNullOrEmpty(data.chainstopperd) Then Return "CHAIN STOPPER FOR FOURTH BLIND IS REQUIRED !"
                If String.IsNullOrEmpty(data.controllengthd) Then Return "CHAIN LENGTH FOR FOURTH BLIND IS REQUIRED !"
                If data.controllengthd = "Custom" Then
                    chainTypeD = orderClass.GetChainType(data.chaincolourd)

                    If chainTypeD = "Continuous" Then
                        If String.IsNullOrEmpty(data.controllengthvalued2) Then Return "CHAIN LENGTH VALUE FOR FOURTH BLIND IS REQUIRED !"
                        If Not Integer.TryParse(data.controllengthvalued2, controllengthd) OrElse controllengthd <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE FOR FOURTH BLIND !"
                    End If
                    If chainTypeD = "Non Continuous" Then
                        If String.IsNullOrEmpty(data.controllengthvalued) Then Return "CHAIN LENGTH VALUE FOR FOURTH BLIND IS REQUIRED !"
                        If Not Integer.TryParse(data.controllengthvalued, controllengthd) OrElse controllengthd <= 0 Then Return "PLEASE CHECK YOUR CHAIN LENGTH VALUE FOR FOURTH BLIND !"
                    End If
                End If
            End If
        End If

        If blindName = "DB Link 2 Blinds Dependent" OrElse blindName = "DB Link 2 Blinds Independent" OrElse blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If Not String.IsNullOrEmpty(data.bottomtyped) Then
                If String.IsNullOrEmpty(data.bottomcolourd) Then Return "BOTTOM COLOUR FOR FOURTH BLIND IS REQUIRED !"
                If bottomNameD = "Flat" AndAlso String.IsNullOrEmpty(data.bottomoptiond) Then Return "FLAT BOTTOM FOR FOURTH BLIND IS REQUIRED !"
            End If

            If String.IsNullOrEmpty(data.widthd) Then Return "WIDTH FOR FOURTH BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.widthd, widthd) OrElse widthd <= 0 Then Return "PLEASE CHECK YOUR WIDTH FOR FOURTH BLIND !"

            If String.IsNullOrEmpty(data.dropd) Then Return "DROP FOR FOURTH BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.dropd, dropd) OrElse dropd <= 0 Then Return "PLEASE CHECK YOUR DROP FOR FOURTH BLIND !"

            squareMetreD = widthd * dropd / 1000000

            If tubeName.Contains("Gear Reduction") Then
                If data.companyid = "2" Then
                    If tubeName = "Gear Reduction 38mm" And widthd > 1810 Then Return "MAXIMUM WIDTH FOR SECOND BLIND GEAR REDUCTION 38MM IS 1810MM !"
                    If squareMetreD >= 6 AndAlso (tubeName = "Gear Reduction 38mm" OrElse tubeName = "Gear Reduction 45mm") Then
                        Return "YOUR BLIND AREA EXCEEDS 6 SQM.<br />PLEASE USE <b>GEAR REDUCTION 49MM</b> IF YOU WISH TO CONTINUE USING THE GEAR REDUCTION SYSTEM.<br />OUR ALTERNATIVE RECOMMENDATION:<br />ACMEDA SYSTEM: <b>ACMEDA 49MM</b><br />SUNBOSS SYSTEM: <b>SUNBOSS 43MM</b> OR <b>SUNBOSS 50MM</b>"
                    End If
                End If
            End If
        End If
        ' END FOURTH BLIND

        ' START FIFTH BLIND
        If blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If Not String.IsNullOrEmpty(data.bottomtypee) Then
                If String.IsNullOrEmpty(data.bottomcoloure) Then Return "BOTTOM COLOUR FOR FIFTH BLIND IS REQUIRED !"
                If bottomNameE = "Flat" Then Return "FLAT BOTTOM FOR FIFTH BLIND IS REQUIRED !"
            End If

            If String.IsNullOrEmpty(data.widthe) Then Return "WIDTH FOR FIFTH BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.widthe, widthe) OrElse widthe <= 0 Then Return "PLEASE CHECK YOUR WIDTH FOR FIFTH BLIND !"

            If String.IsNullOrEmpty(data.drope) Then Return "DROP FOR FIFTH BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.drope, drope) OrElse drope <= 0 Then Return "PLEASE CHECK YOUR DROP FOR FIFTH BLIND !"

            squareMetreE = widthe * drope / 1000000

            If tubeName.Contains("Gear Reduction") Then
                If tubeName = "Gear Reduction 38mm" And widthe > 1810 Then Return "MINIMUM WIDTH FOR SECOND BLIND GEAR REDUCTION 38MM IS 1810MM !"
                If squareMetreE >= 6 AndAlso (tubeName = "Gear Reduction 38mm" OrElse tubeName = "Gear Reduction 45mm") Then
                    Return "YOUR BLIND AREA EXCEEDS 6 SQM.<br />PLEASE USE <b>GEAR REDUCTION 49MM</b> IF YOU WISH TO CONTINUE USING THE GEAR REDUCTION SYSTEM.<br />OUR ALTERNATIVE RECOMMENDATION:<br />ACMEDA SYSTEM: <b>ACMEDA 49MM</b><br />SUNBOSS SYSTEM: <b>SUNBOSS 43MM</b> OR <b>SUNBOSS 50MM</b>"
                End If
            End If
        End If
        ' END FIFTH BLIND

        ' START SIXTH BLIND
        If blindName = "DB Link 3 Blinds Dependent" OrElse blindName = "DB Link 3 Blinds Independent with Dependent" Then
            If Not String.IsNullOrEmpty(data.bottomtypef) Then
                If String.IsNullOrEmpty(data.bottomcolourf) Then Return "BOTTOM COLOUR FOR SIXTH BLIND IS REQUIRED !"
                If bottomNameF = "Flat" Then Return "FLAT BOTTOM FOR SIXTH BLIND IS REQUIRED !"
            End If

            If String.IsNullOrEmpty(data.widthf) Then Return "WIDTH FOR SIXTH BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.widthf, widthf) OrElse widthf <= 0 Then Return "PLEASE CHECK YOUR WIDTH FOR SIXTH BLIND !"

            If String.IsNullOrEmpty(data.dropf) Then Return "DROP FOR SIXTH BLIND IS REQUIRED !"
            If Not Integer.TryParse(data.dropf, dropf) OrElse dropf <= 0 Then Return "PLEASE CHECK YOUR DROP FOR SIXTH BLIND !"

            squareMetreF = widthf * dropf / 1000000

            If tubeName.Contains("Gear Reduction") Then
                If tubeName = "Gear Reduction 38mm" And widthe > 1810 Then Return "MINIMUM WIDTH FOR SECOND BLIND GEAR REDUCTION 38MM IS 1810MM !"
                If squareMetreE >= 6 AndAlso (tubeName = "Gear Reduction 38mm" OrElse tubeName = "Gear Reduction 45mm") Then
                    Return "YOUR BLIND AREA EXCEEDS 6 SQM.<br />PLEASE USE <b>GEAR REDUCTION 49MM</b> IF YOU WISH TO CONTINUE USING THE GEAR REDUCTION SYSTEM.<br />OUR ALTERNATIVE RECOMMENDATION:<br />ACMEDA SYSTEM: <b>ACMEDA 49MM</b><br />SUNBOSS SYSTEM: <b>SUNBOSS 43MM</b> OR <b>SUNBOSS 50MM</b>"
                End If
            End If
        End If
        ' END SIXTH BLIND

        If tubeName.Contains("Sunboss") Then
            If String.IsNullOrEmpty(data.bracketsize) Then Return "SUNBOSS BRACKET SIZE IS REQUIRED !"
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        Dim priceProductGroup As String = String.Empty
        Dim priceProductGroupB As String = String.Empty
        Dim priceProductGroupC As String = String.Empty
        Dim priceProductGroupD As String = String.Empty
        Dim priceProductGroupE As String = String.Empty
        Dim priceProductGroupF As String = String.Empty

        Dim groupFabric As String = String.Empty
        Dim groupFabricDB As String = String.Empty

        ' CASSETTE CRUD
        If blindName = "Full Cassette" OrElse blindName = "Semi Cassette" Then
            data.roll = "Standard"
            data.bottomtype = String.Empty : data.bottomcolour = String.Empty
            data.bottomoption = String.Empty

            data.fabrictypeb = String.Empty : data.fabriccolourb = String.Empty
            data.rollb = String.Empty : data.controlpositionb = String.Empty
            data.bottomtypeb = String.Empty : data.bottomcolourb = String.Empty
            data.bottomoptionb = String.Empty
            widthb = 0 : dropb = 0

            data.fabrictypec = String.Empty : data.fabriccolourc = String.Empty
            data.rollc = String.Empty : data.controlpositionc = String.Empty
            data.bottomtypec = String.Empty : data.bottomcolourc = String.Empty
            data.bottomoptionc = String.Empty
            widthc = 0 : dropc = 0

            data.fabrictyped = String.Empty : data.fabriccolourd = String.Empty
            data.rolld = String.Empty : data.controlpositiond = String.Empty
            data.bottomtyped = String.Empty : data.bottomcolourd = String.Empty
            data.bottomoptiond = String.Empty
            widthd = 0 : dropd = 0

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.printingb = String.Empty
            data.printingc = String.Empty
            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If

            data.adjusting = String.Empty
            data.bracketextension = String.Empty

            If controlName = "Chain" Then
                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If
            End If

            linearMetre = width / 1000
            squareMetre = width * drop / 1000000

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)
            If data.companyid = "3" Then
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
            End If

            Dim tubeIstilah As String = "Standard"

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
        End If

        ' WIRE GUIDE CRUD
        If blindName = "Wire Guide" Then
            data.roll = "Standard"

            data.fabrictypeb = String.Empty : data.fabriccolourb = String.Empty
            data.rollb = String.Empty : data.controlpositionb = String.Empty
            data.bottomtypeb = String.Empty : data.bottomcolourb = String.Empty
            data.bottomoptionb = String.Empty
            widthb = 0 : dropb = 0

            data.fabrictypec = String.Empty : data.fabriccolourc = String.Empty
            data.rollc = String.Empty : data.controlpositionc = String.Empty
            data.bottomtypec = String.Empty : data.bottomcolourc = String.Empty
            data.bottomoptionc = String.Empty
            widthc = 0 : dropc = 0

            data.fabrictyped = String.Empty : data.fabriccolourd = String.Empty
            data.rolld = String.Empty : data.controlpositiond = String.Empty
            data.bottomtyped = String.Empty : data.bottomcolourd = String.Empty
            data.bottomoptiond = String.Empty
            widthd = 0 : dropd = 0

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.printingb = String.Empty
            data.printingc = String.Empty
            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If

            data.adjusting = String.Empty
            data.bracketextension = String.Empty

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chainstopper = String.Empty
                data.controllength = String.Empty
                controllength = 0
                data.springassist = String.Empty
            End If

            If controlName = "Chain" Then
                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = String.Empty
            End If
            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"

            linearMetre = width / 1000
            squareMetre = width * drop / 1000000

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)

            Dim tubeIstilah As String = "Standard"

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
        End If

        ' REGULAR CRUD
        If blindName = "Single Blind" Then
            data.fabrictypeb = String.Empty : data.fabriccolourb = String.Empty
            data.rollb = String.Empty : data.controlpositionb = String.Empty
            data.bottomtypeb = String.Empty : data.bottomcolourb = String.Empty
            data.bottomoptionb = String.Empty
            widthb = 0 : dropb = 0

            data.fabrictypec = String.Empty : data.fabriccolourc = String.Empty
            data.rollc = String.Empty : data.controlpositionc = String.Empty
            data.bottomtypec = String.Empty : data.bottomcolourc = String.Empty
            data.bottomoptionc = String.Empty
            widthc = 0 : dropc = 0

            data.fabrictyped = String.Empty : data.fabriccolourd = String.Empty
            data.rolld = String.Empty : data.controlpositiond = String.Empty
            data.bottomtyped = String.Empty : data.bottomcolourd = String.Empty
            data.bottomoptiond = String.Empty
            widthd = 0 : dropd = 0

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.printingb = String.Empty
            data.printingc = String.Empty
            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If

            data.adjusting = String.Empty

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chainstopper = String.Empty
                data.controllength = String.Empty
                controllength = 0
                data.springassist = String.Empty
            End If

            If controlName = "Chain" Then
                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If
                If Not tubeName = "Acmeda 49mm" Then data.springassist = String.Empty
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = String.Empty
            End If
            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"

            linearMetre = width / 1000
            squareMetre = width * drop / 1000000

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)
            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
        End If

        ' Dual Blinds CRUD
        If blindName = "Dual Blinds" Then
            data.fabrictypec = String.Empty : data.fabriccolourc = String.Empty
            data.rollc = String.Empty : data.controlpositionc = String.Empty
            data.bottomtypec = String.Empty : data.bottomcolourc = String.Empty
            data.bottomoptionc = String.Empty
            widthc = 0 : dropc = 0

            data.fabrictyped = String.Empty : data.fabriccolourd = String.Empty
            data.rolld = String.Empty : data.controlpositiond = String.Empty
            data.bottomtyped = String.Empty : data.bottomcolourd = String.Empty
            data.bottomoptiond = String.Empty
            widthd = 0 : dropd = 0

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.toptrack = String.Empty
            data.adjusting = String.Empty

            data.printingc = String.Empty
            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            data.bracketextension = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                orderClass.DeleteFilePrinting(data.orderid, data.printingb)

                data.printing = String.Empty
                data.printingb = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chainstopper = String.Empty : data.chainstopperb = String.Empty
                data.controllength = String.Empty : data.controllengthb = String.Empty
                controllength = 0 : controllengthb = 0
            End If

            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.chaincolourb) Then
                    data.chaincolourb = data.chaincolour
                    data.controllengthb = data.controllength
                    If data.controllength = "Custom" Then
                        controllengthb = controllength
                    End If
                    data.controllengthvalueb = data.controllengthvalue
                    chainTypeB = orderClass.GetChainType(data.chaincolourb)
                End If
                If String.IsNullOrEmpty(data.chainstopperb) Then
                    data.chainstopperb = data.chainstopper
                End If

                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If

                If data.controllengthb = "" OrElse data.controllengthb = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropb * 2 / 3)
                    If chainTypeB = "Non Continuous" Then
                        controllengthb = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthb = Math.Ceiling((dropb * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeB = "Continuous" Then
                        controllengthb = 500
                        If stdControlLength > 500 Then controllengthb = 750
                        If stdControlLength > 750 Then controllengthb = 1000
                        If stdControlLength > 1000 Then controllengthb = 1200
                        If stdControlLength > 1200 Then controllengthb = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If String.IsNullOrEmpty(data.bottomtypeb) Then
                data.bottomtypeb = data.bottomtype
                data.bottomcolourb = data.bottomcolour
                data.bottomoptionb = data.bottomoption
                bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = ""
            End If
            If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                data.bottomoptionb = ""
            End If

            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
            If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000

            totalItems = 2

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)
            groupFabricDB = orderClass.GetFabricGroup(data.fabrictypeb)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
                groupFabricDB = orderClass.GetFabricGroupLocal("Roller", data.fabrictypeb)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            Dim groupNameDB As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabricDB)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
        End If

        If blindName = "Link 2 Blinds Dependent" Then
            data.fabrictypeb = data.fabrictype
            data.fabriccolourb = data.fabriccolour
            data.rollb = data.roll
            data.controlpositionb = String.Empty
            data.chaincolourb = String.Empty : data.chainstopperb = String.Empty
            data.controllengthb = String.Empty : controllengthb = 0

            data.fabrictypec = String.Empty : data.fabriccolourc = String.Empty
            data.rollc = String.Empty : data.controlpositionc = String.Empty
            data.bottomtypec = String.Empty : data.bottomcolourc = String.Empty
            data.bottomoptionc = String.Empty
            widthc = 0 : dropc = 0

            data.fabrictyped = String.Empty : data.fabriccolourd = String.Empty
            data.rolld = String.Empty : data.controlpositiond = String.Empty
            data.bottomtyped = String.Empty : data.bottomcolourd = String.Empty
            data.bottomoptiond = String.Empty
            widthd = 0 : dropd = 0

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.toptrack = String.Empty

            data.printingc = String.Empty
            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If
            If widthb > 1510 OrElse dropb > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingb)
                data.printingb = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chainstopper = String.Empty
                data.controllength = String.Empty
                controllength = 0
            End If

            If controlName = "Chain" Then
                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If String.IsNullOrEmpty(data.bottomtypeb) Then
                data.bottomtypeb = data.bottomtype
                data.bottomcolourb = data.bottomcolour
                data.bottomoptionb = data.bottomoption
                bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = ""
            End If
            If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                data.bottomoptionb = ""
            End If

            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
            If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"

            If tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Acmeda") OrElse tubeName = "Standard" Then
                data.adjusting = String.Empty
            End If

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000

            totalItems = 2

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
        End If

        If blindName = "Link 2 Blinds Independent" Then
            data.fabrictypeb = data.fabrictype
            data.fabriccolourb = data.fabriccolour
            data.rollb = data.roll
            data.controlposition = "Left" : data.controlpositionb = "Right"

            data.fabrictypec = String.Empty : data.fabriccolourc = String.Empty
            data.rollc = String.Empty : data.controlpositionc = String.Empty
            data.bottomtypec = String.Empty : data.bottomcolourc = String.Empty
            data.bottomoptionc = String.Empty
            widthc = 0 : dropc = 0

            data.fabrictyped = String.Empty : data.fabriccolourd = String.Empty
            data.rolld = String.Empty : data.controlpositiond = String.Empty
            data.bottomtyped = String.Empty : data.bottomcolourd = String.Empty
            data.bottomoptiond = String.Empty
            widthd = 0 : dropd = 0

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.toptrack = String.Empty

            data.printingc = String.Empty
            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If
            If widthb > 1510 OrElse dropb > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingb)
                data.printingb = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chaincolourb = String.Empty
                data.chainstopper = String.Empty : data.chainstopperb = String.Empty
                data.controllength = String.Empty : data.controllengthb = String.Empty
                controllength = 0 : controllengthb = 0
            End If

            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.chaincolourb) Then
                    data.chaincolourb = data.chaincolour
                    data.controllengthb = data.controllength
                    data.controllengthvalueb = data.controllengthvalue
                    chainTypeB = orderClass.GetChainType(data.chaincolourb)
                End If
                If String.IsNullOrEmpty(data.chainstopperb) Then
                    data.chainstopperb = data.chainstopper
                End If

                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If

                If data.controllengthb = "" OrElse data.controllengthb = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropb * 2 / 3)
                    If chainTypeB = "Non Continuous" Then
                        controllengthb = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthb = Math.Ceiling((dropb * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeB = "Continuous" Then
                        controllengthb = 500
                        If stdControlLength > 500 Then controllengthb = 750
                        If stdControlLength > 750 Then controllengthb = 1000
                        If stdControlLength > 1000 Then controllengthb = 1200
                        If stdControlLength > 1200 Then controllengthb = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If String.IsNullOrEmpty(data.bottomtypeb) Then
                data.bottomtypeb = data.bottomtype
                data.bottomcolourb = data.bottomcolour
                data.bottomoptionb = data.bottomoption
                bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = ""
            End If
            If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                data.bottomoptionb = ""
            End If

            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
            If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"

            If tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Acmeda") OrElse tubeName = "Standard" Then
                data.adjusting = String.Empty
            End If

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000

            totalItems = 2

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
        End If

        If blindName = "Link 3 Blinds Dependent" Then
            data.fabrictypeb = data.fabrictype
            data.fabriccolourb = data.fabriccolour
            data.rollb = data.roll
            data.controlpositionb = String.Empty
            data.chaincolourb = String.Empty : data.chainstopperb = String.Empty
            data.controllengthb = String.Empty : controllengthb = 0

            data.fabrictypec = data.fabrictype
            data.fabriccolourc = data.fabriccolour
            data.rollc = data.roll
            data.controlpositionc = String.Empty
            data.chaincolourc = String.Empty
            data.chainstopperc = String.Empty
            data.controllengthc = String.Empty
            controllengthc = 0

            data.fabrictyped = String.Empty : data.fabriccolourd = String.Empty
            data.rolld = String.Empty : data.controlpositiond = String.Empty
            data.bottomtyped = String.Empty : data.bottomcolourd = String.Empty
            data.bottomoptiond = String.Empty
            widthd = 0 : dropd = 0

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.toptrack = String.Empty

            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If
            If widthb > 1510 OrElse dropb > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingb)
                data.printingb = String.Empty
            End If
            If widthc > 1510 OrElse dropc > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingc)
                data.printingc = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chainstopper = String.Empty
                data.controllength = String.Empty
                controllength = 0
            End If

            If controlName = "Chain" Then
                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If String.IsNullOrEmpty(data.bottomtypeb) Then
                data.bottomtypeb = data.bottomtype
                data.bottomcolourb = data.bottomcolour
                data.bottomoptionb = data.bottomoption
                bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
            End If
            If String.IsNullOrEmpty(data.bottomtypec) Then
                data.bottomtypec = data.bottomtype
                data.bottomcolourc = data.bottomcolour
                data.bottomoptionc = data.bottomoption
                bottomNameC = orderClass.GetBottomName(data.bottomtypec)
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = ""
            End If
            If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                data.bottomoptionb = ""
            End If
            If Not (bottomNameC = "Flat" OrElse bottomNameC = "Flat Mohair") Then
                data.bottomoptionc = ""
            End If

            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
            If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"
            If bottomNameC = "Flat Mohair" Then data.bottomoptionc = "Fabric on Front"

            If tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Acmeda") OrElse tubeName = "Standard" Then
                data.adjusting = String.Empty
            End If

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            linearMetreC = widthc / 1000
            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000
            squareMetreC = widthc * dropc / 1000000

            totalItems = 3

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupC = orderClass.GetPriceProductGroupId(groupName, data.designid)
        End If

        If blindName = "Link 3 Blinds Independent with Dependent" Then
            data.fabrictypeb = data.fabrictype
            data.fabriccolourb = data.fabriccolour
            data.rollb = data.roll
            If data.controlposition = "Left" Then
                data.controlpositionb = String.Empty
                data.controlpositionc = "Right"
            End If
            If data.controlposition = "Right" Then
                data.controlpositionb = String.Empty
                data.controlpositionc = "Left"
            End If
            data.chaincolourb = String.Empty : data.chainstopperb = String.Empty
            data.controllengthb = String.Empty : controllengthb = 0

            data.fabrictypec = data.fabrictype
            data.fabriccolourc = data.fabriccolour
            data.rollc = data.roll

            data.fabrictyped = String.Empty : data.fabriccolourd = String.Empty
            data.rolld = String.Empty : data.controlpositiond = String.Empty
            data.bottomtyped = String.Empty : data.bottomcolourd = String.Empty
            data.bottomoptiond = String.Empty
            widthd = 0 : dropd = 0

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.toptrack = String.Empty

            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If
            If widthb > 1510 OrElse dropb > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingb)
                data.printingb = String.Empty
            End If
            If widthc > 1510 OrElse dropc > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingc)
                data.printingc = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chainstopper = String.Empty : data.chainstopperc = String.Empty
                data.controllength = String.Empty : data.controllengthc = String.Empty
                controllength = 0 : controllengthc = 0
            End If

            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.chaincolourc) Then
                    data.chaincolourc = data.chaincolour
                    data.controllengthc = data.controllength
                    chainTypeC = orderClass.GetChainType(data.chaincolourc)
                End If
                If String.IsNullOrEmpty(data.chainstopperc) Then
                    data.chainstopperc = data.chainstopper
                End If

                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If

                If data.controllengthc = "" OrElse data.controllengthc = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropc * 2 / 3)
                    If chainTypeC = "Non Continuous" Then
                        controllengthc = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthc = Math.Ceiling((dropc * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeC = "Continuous" Then
                        controllengthc = 500
                        If stdControlLength > 500 Then controllengthc = 750
                        If stdControlLength > 750 Then controllengthc = 1000
                        If stdControlLength > 1000 Then controllengthc = 1200
                        If stdControlLength > 1200 Then controllengthc = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If String.IsNullOrEmpty(data.bottomtypeb) Then
                data.bottomtypeb = data.bottomtype
                data.bottomcolourb = data.bottomcolour
                data.bottomoptionb = data.bottomoption
                bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
            End If

            If String.IsNullOrEmpty(data.bottomtypec) Then
                data.bottomtypec = data.bottomtype
                data.bottomcolourc = data.bottomcolour
                data.bottomoptionc = data.bottomoption
                bottomNameC = orderClass.GetBottomName(data.bottomtypec)
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = ""
            End If
            If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                data.bottomoptionb = ""
            End If
            If Not (bottomNameC = "Flat" OrElse bottomNameC = "Flat Mohair") Then
                data.bottomoptionc = ""
            End If

            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
            If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"
            If bottomNameC = "Flat Mohair" Then data.bottomoptionc = "Fabric on Front"

            If tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Acmeda") OrElse tubeName = "Standard" Then
                data.adjusting = String.Empty
            End If

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            linearMetreC = widthc / 1000
            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000
            squareMetreC = widthc * dropc / 1000000

            totalItems = 3

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupC = orderClass.GetPriceProductGroupId(groupName, data.designid)
        End If

        If blindName = "DB Link 2 Blinds Dependent" Then
            data.fabrictypeb = data.fabrictype
            data.fabriccolourb = data.fabriccolour
            data.rollb = data.roll
            data.controlpositionb = String.Empty
            data.chaincolourb = String.Empty : data.chainstopperb = String.Empty
            data.controllengthb = String.Empty : controllengthb = 0

            data.fabrictyped = data.fabrictypec
            data.fabriccolourd = data.fabriccolourc
            data.rolld = data.roll : data.controlpositiond = String.Empty

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.toptrack = String.Empty
            data.bracketextension = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If
            If widthb > 1510 OrElse dropb > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingb)
                data.printingb = String.Empty
            End If
            If widthc > 1510 OrElse dropc > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingc)
                data.printingc = String.Empty
            End If
            If widthd > 1510 OrElse dropd > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingd)
                data.printingd = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chaincolourc = String.Empty
                data.chainstopper = String.Empty : data.chainstopperc = String.Empty
                data.controllength = String.Empty : data.controllengthc = String.Empty
                controllength = 0 : controllengthc = 0
            End If

            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.chaincolourc) Then
                    data.chaincolourc = data.chaincolour
                    data.chainstopperc = data.chainstopper
                    data.controllengthc = data.controllength
                    chainTypeC = orderClass.GetChainType(data.chaincolourc)
                End If
                If String.IsNullOrEmpty(data.chainstopperc) Then
                    data.chainstopperc = data.chainstopper
                End If

                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If

                If data.controllengthc = "" OrElse data.controllengthc = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropc * 2 / 3)
                    If chainTypeC = "Non Continuous" Then
                        controllengthc = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthc = Math.Ceiling((dropc * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeC = "Continuous" Then
                        controllengthc = 500
                        If stdControlLength > 500 Then controllengthc = 750
                        If stdControlLength > 750 Then controllengthc = 1000
                        If stdControlLength > 1000 Then controllengthc = 1200
                        If stdControlLength > 1200 Then controllengthc = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If String.IsNullOrEmpty(data.bottomtypeb) Then
                data.bottomtypeb = data.bottomtype
                data.bottomcolourb = data.bottomcolour
                data.bottomoptionb = data.bottomoption
                bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
            End If

            If String.IsNullOrEmpty(data.bottomtypec) Then
                data.bottomtypec = data.bottomtype
                data.bottomcolourc = data.bottomcolour
                data.bottomoptionc = data.bottomoption
                bottomNameC = orderClass.GetBottomName(data.bottomtypec)
            End If

            If String.IsNullOrEmpty(data.bottomtyped) Then
                data.bottomtyped = data.bottomtype
                data.bottomcolourd = data.bottomcolour
                data.bottomoptiond = data.bottomoption
                bottomNameD = orderClass.GetBottomName(data.bottomtyped)
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = ""
            End If
            If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                data.bottomoptionb = ""
            End If
            If Not (bottomNameC = "Flat" OrElse bottomNameC = "Flat Mohair") Then
                data.bottomoptionc = ""
            End If
            If Not (bottomNameD = "Flat" OrElse bottomNameD = "Flat Mohair") Then
                data.bottomoptiond = ""
            End If

            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
            If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"
            If bottomNameC = "Flat Mohair" Then data.bottomoptionc = "Fabric on Front"
            If bottomNameD = "Flat Mohair" Then data.bottomoptiond = "Fabric on Front"

            If tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Acmeda") OrElse tubeName = "Standard" Then
                data.adjusting = String.Empty
            End If

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            linearMetreC = widthc / 1000
            linearMetreC = widthd / 1000
            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000
            squareMetreC = widthc * dropc / 1000000
            squareMetreD = widthd * dropd / 1000000

            totalItems = 4

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)
            groupFabricDB = orderClass.GetFabricGroup(data.fabrictypec)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
                groupFabricDB = orderClass.GetFabricGroupLocal("Roller", data.fabrictypec)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            Dim groupNameDB As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabricDB)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupC = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
            priceProductGroupD = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
        End If

        If blindName = "DB Link 2 Blinds Independent" Then
            data.fabrictypeb = data.fabrictype
            data.fabriccolourb = data.fabriccolour
            data.rollb = data.roll
            data.controlposition = "Left" : data.controlpositionb = "Right"

            data.controlpositionc = "Left" : data.controlpositiond = "Right"

            data.fabrictyped = data.fabrictypec : data.fabriccolourd = data.fabriccolourc
            data.rolld = data.rollc

            data.fabrictypee = String.Empty : data.fabriccoloure = String.Empty
            data.rolle = String.Empty : data.controlpositione = String.Empty
            data.bottomtypee = String.Empty : data.bottomcoloure = String.Empty
            data.bottomoptione = String.Empty
            widthe = 0 : drope = 0

            data.fabrictypef = String.Empty : data.fabriccolourf = String.Empty
            data.rollf = String.Empty : data.controlpositionf = String.Empty
            data.bottomtypef = String.Empty : data.bottomcolourf = String.Empty
            data.bottomoptionf = String.Empty
            widthf = 0 : dropf = 0

            data.toptrack = String.Empty
            data.bracketextension = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If
            If widthb > 1510 OrElse dropb > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingb)
                data.printingb = String.Empty
            End If
            If widthc > 1510 OrElse dropc > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingc)
                data.printingc = String.Empty
            End If
            If widthd > 1510 OrElse dropd > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingd)
                data.printingd = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote : data.chaincolourb = String.Empty
                data.chaincolourc = String.Empty : data.chaincolourd = String.Empty

                data.chainstopper = String.Empty : data.chainstopperb = String.Empty
                data.chainstopperc = String.Empty : data.chainstopperd = String.Empty

                data.controllength = String.Empty : data.controllengthb = String.Empty
                data.controllengthc = String.Empty : data.controllengthd = String.Empty

                controllength = 0 : controllengthb = 0
                controllengthc = 0 : controllengthd = 0
            End If

            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.chaincolourb) Then
                    data.chaincolourb = data.chaincolour
                    data.controllengthb = data.controllength
                    chainTypeB = orderClass.GetChainType(data.chaincolourb)
                End If
                If String.IsNullOrEmpty(data.chainstopperb) Then
                    data.chainstopperb = data.chainstopper
                End If

                If String.IsNullOrEmpty(data.chaincolourc) Then
                    data.chaincolourc = data.chaincolour
                    data.controllengthc = data.controllength
                    chainTypeC = orderClass.GetChainType(data.chaincolourc)
                End If
                If String.IsNullOrEmpty(data.chainstopperc) Then
                    data.chainstopperc = data.chainstopper
                End If

                If String.IsNullOrEmpty(data.chaincolourd) Then
                    data.chaincolourd = data.chaincolour
                    data.controllengthd = data.controllength
                    chainTypeD = orderClass.GetChainType(data.chaincolourd)
                End If
                If String.IsNullOrEmpty(data.chainstopperd) Then
                    data.chainstopperd = data.chainstopper
                End If

                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If

                If data.controllengthb = "" OrElse data.controllengthb = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropb * 2 / 3)
                    If chainTypeB = "Non Continuous" Then
                        controllengthb = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthb = Math.Ceiling((dropb * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeB = "Continuous" Then
                        controllengthb = 500
                        If stdControlLength > 500 Then controllengthb = 750
                        If stdControlLength > 750 Then controllengthb = 1000
                        If stdControlLength > 1000 Then controllengthb = 1200
                        If stdControlLength > 1200 Then controllengthb = 1500
                    End If
                End If

                If data.controllengthc = "Standard" Then
                    Dim stdControlLengthC As Integer = Math.Ceiling(dropc * 2 / 3)
                    If chainTypeC = "Non Continuous" Then
                        controllengthc = stdControlLengthC
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthc = Math.Ceiling((dropc * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeC = "Continuous" Then
                        controllengthc = 500
                        If stdControlLengthC > 500 Then controllengthc = 750
                        If stdControlLengthC > 750 Then controllengthc = 1000
                        If stdControlLengthC > 1000 Then controllengthc = 1200
                        If stdControlLengthC > 1200 Then controllengthc = 1500
                    End If
                End If

                If data.controllengthd = "Standard" Then
                    Dim stdControlLengthD As Integer = Math.Ceiling(dropd * 2 / 3)
                    If chainTypeD = "Non Continuous" Then
                        controllengthd = stdControlLengthD
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthd = Math.Ceiling((dropd * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeD = "Continuous" Then
                        controllengthd = 500
                        If stdControlLengthD > 500 Then controllengthd = 750
                        If stdControlLengthD > 750 Then controllengthd = 1000
                        If stdControlLengthD > 1000 Then controllengthd = 1200
                        If stdControlLengthD > 1200 Then controllengthd = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If String.IsNullOrEmpty(data.bottomtypeb) Then
                data.bottomtypeb = data.bottomtype
                data.bottomcolourb = data.bottomcolour
                data.bottomoptionb = data.bottomoption
                bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
            End If

            If String.IsNullOrEmpty(data.bottomtypec) Then
                data.bottomtypec = data.bottomtype
                data.bottomcolourc = data.bottomcolour
                data.bottomoptionc = data.bottomoption
                bottomNameC = orderClass.GetBottomName(data.bottomtypec)
            End If

            If String.IsNullOrEmpty(data.bottomtyped) Then
                data.bottomtyped = data.bottomtype
                data.bottomcolourd = data.bottomcolour
                data.bottomoptiond = data.bottomoption
                bottomNameD = orderClass.GetBottomName(data.bottomtyped)
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = ""
            End If
            If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                data.bottomoptionb = ""
            End If
            If Not (bottomNameC = "Flat" OrElse bottomNameC = "Flat Mohair") Then
                data.bottomoptionc = ""
            End If
            If Not (bottomNameD = "Flat" OrElse bottomNameD = "Flat Mohair") Then
                data.bottomoptiond = ""
            End If

            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
            If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"
            If bottomNameC = "Flat Mohair" Then data.bottomoptionc = "Fabric on Front"
            If bottomNameD = "Flat Mohair" Then data.bottomoptiond = "Fabric on Front"

            If tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Acmeda") OrElse tubeName = "Standard" Then
                data.adjusting = String.Empty
            End If

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            linearMetreC = widthc / 1000
            linearMetreD = widthd / 1000

            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000
            squareMetreC = widthc * dropc / 1000000
            squareMetreD = widthd * dropd / 1000000

            totalItems = 4

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)
            groupFabricDB = orderClass.GetFabricGroup(data.fabrictypec)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
                groupFabricDB = orderClass.GetFabricGroupLocal("Roller", data.fabrictypec)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            Dim groupNameDB As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabricDB)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupC = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
            priceProductGroupD = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
        End If

        If blindName = "DB Link 3 Blinds Dependent" Then
            data.fabrictypeb = data.fabrictype
            data.fabrictypec = data.fabrictype
            data.fabriccolourb = data.fabriccolour
            data.fabriccolourc = data.fabriccolour
            data.rollb = data.roll
            data.rollc = data.roll
            data.controlpositionb = String.Empty
            data.controlpositionc = String.Empty
            data.chaincolourb = String.Empty : data.chainstopperb = String.Empty
            data.chaincolourc = String.Empty : data.chainstopperc = String.Empty
            data.controllengthb = String.Empty : controllengthb = 0
            data.controllengthc = String.Empty : controllengthc = 0

            data.fabrictypee = data.fabrictyped
            data.fabrictypef = data.fabrictyped
            data.fabriccoloure = data.fabriccolourd
            data.fabriccolourf = data.fabriccolourd
            data.chaincoloure = String.Empty : data.chainstoppere = String.Empty
            data.chaincolourf = String.Empty : data.chainstopperf = String.Empty
            data.controllengthe = String.Empty : controllengthe = 0
            data.controllengthf = String.Empty : controllengthf = 0
            data.rolle = data.rolld
            data.rollf = data.rolld
            data.controlpositione = String.Empty
            data.controlpositionf = String.Empty

            data.toptrack = String.Empty
            data.bracketextension = String.Empty

            If width > 1510 AndAlso drop > 1510 Then
                data.printing = String.Empty
            End If
            If widthb > 1510 AndAlso dropb > 1510 Then
                data.printingb = String.Empty
            End If
            If widthc > 1510 AndAlso dropc > 1510 Then
                data.printingc = String.Empty
            End If
            If widthd > 1510 AndAlso dropd > 1510 Then
                data.printingd = String.Empty
            End If
            If widthe > 1510 AndAlso drope > 1510 Then
                data.printinge = String.Empty
            End If
            If widthf > 1510 AndAlso dropf > 1510 Then
                data.printingf = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "Sonesse 30 WF" OrElse controlName = "LSN40" Then
                data.chaincolour = data.remote
                data.chaincolourd = String.Empty
                data.chainstopper = String.Empty : data.chainstopperd = String.Empty
                data.controllength = String.Empty : data.controllengthd = String.Empty
                controllength = 0 : controllengthd = 0
            End If

            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.chaincolourd) Then
                    data.chaincolourd = data.chaincolour
                    data.chainstopperd = data.chainstopper
                    data.controllengthd = data.controllength
                    chainTypeD = orderClass.GetChainType(data.chaincolourd)
                End If
                If String.IsNullOrEmpty(data.chainstopperd) Then
                    data.chainstopperd = data.chainstopper
                End If

                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If

                If data.controllengthd = "" OrElse data.controllengthd = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropd * 2 / 3)
                    If chainTypeD = "Non Continuous" Then
                        controllengthd = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthd = Math.Ceiling((dropd * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeD = "Continuous" Then
                        controllengthd = 500
                        If stdControlLength > 500 Then controllengthd = 750
                        If stdControlLength > 750 Then controllengthd = 1000
                        If stdControlLength > 1000 Then controllengthd = 1200
                        If stdControlLength > 1200 Then controllengthd = 1500
                    End If
                End If
            End If

            If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                data.charger = String.Empty
            End If

            If Not controlName.Contains("Alpha") Then
                data.extensioncable = String.Empty
                data.supply = String.Empty
            End If

            If data.companyid = "2" Then data.drycontact = String.Empty
            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                data.drycontact = String.Empty
            End If

            If String.IsNullOrEmpty(data.bottomtypeb) Then
                data.bottomtypeb = data.bottomtype
                data.bottomcolourb = data.bottomcolour
                data.bottomoptionb = data.bottomoption
                bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
            End If

            If String.IsNullOrEmpty(data.bottomtypec) Then
                data.bottomtypec = data.bottomtype
                data.bottomcolourc = data.bottomcolour
                data.bottomoptionc = data.bottomoption
                bottomNameC = orderClass.GetBottomName(data.bottomtypec)
            End If

            If String.IsNullOrEmpty(data.bottomtyped) Then
                data.bottomtyped = data.bottomtype
                data.bottomcolourd = data.bottomcolour
                data.bottomoptiond = data.bottomoption
                bottomNameD = orderClass.GetBottomName(data.bottomtyped)
            End If

            If String.IsNullOrEmpty(data.bottomtypee) Then
                data.bottomtypee = data.bottomtype
                data.bottomcoloure = data.bottomcolour
                data.bottomoptione = data.bottomoption
                bottomNameE = orderClass.GetBottomName(data.bottomtypee)
            End If

            If String.IsNullOrEmpty(data.bottomtypef) Then
                data.bottomtypef = data.bottomtype
                data.bottomcolourf = data.bottomcolour
                data.bottomoptionf = data.bottomoption
                bottomNameF = orderClass.GetBottomName(data.bottomtypef)
            End If

            If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                data.bottomoption = ""
            End If
            If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                data.bottomoptionb = ""
            End If
            If Not (bottomNameC = "Flat" OrElse bottomNameC = "Flat Mohair") Then
                data.bottomoptionc = ""
            End If
            If Not (bottomNameD = "Flat" OrElse bottomNameD = "Flat Mohair") Then
                data.bottomoptiond = ""
            End If
            If Not (bottomNameE = "Flat" OrElse bottomNameE = "Flat Mohair") Then
                data.bottomoptione = ""
            End If
            If Not (bottomNameF = "Flat" OrElse bottomNameF = "Flat Mohair") Then
                data.bottomoptionf = ""
            End If

            If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
            If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"
            If bottomNameC = "Flat Mohair" Then data.bottomoptionc = "Fabric on Front"
            If bottomNameD = "Flat Mohair" Then data.bottomoptiond = "Fabric on Front"
            If bottomNameE = "Flat Mohair" Then data.bottomoptione = "Fabric on Front"
            If bottomNameF = "Flat Mohair" Then data.bottomoptionf = "Fabric on Front"

            If tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Acmeda") OrElse tubeName = "Standard" Then
                data.adjusting = String.Empty
            End If

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            linearMetreC = widthc / 1000
            linearMetreC = widthd / 1000
            linearMetreE = widthe / 1000
            linearMetreF = widthf / 1000

            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000
            squareMetreC = widthc * dropc / 1000000
            squareMetreD = widthd * dropd / 1000000
            squareMetreE = widthe * drope / 1000000
            squareMetreF = widthf * dropf / 1000000

            totalItems = 6

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)
            groupFabricDB = orderClass.GetFabricGroup(data.fabrictyped)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
                groupFabricDB = orderClass.GetFabricGroupLocal("Roller", data.fabrictyped)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            Dim groupNameDB As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabricDB)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupC = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupD = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
            priceProductGroupE = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
            priceProductGroupF = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
        End If

        If blindName = "DB Link 3 Blinds Independent with Dependent" Then
            data.fabrictypeb = data.fabrictype : data.fabriccolourb = data.fabriccolour
            data.rollb = data.roll : data.controlpositionb = String.Empty
            data.chaincolourb = String.Empty : data.chainstopperb = String.Empty
            data.controllengthb = String.Empty : controllengthb = 0

            data.fabrictypec = data.fabrictype : data.fabriccolourc = data.fabriccolour
            data.rollc = data.roll

            data.fabrictypee = data.fabrictyped : data.fabriccoloure = data.fabriccolourd
            data.rolle = data.rolld : data.controlpositione = String.Empty
            data.chaincoloure = String.Empty : data.chainstoppere = String.Empty
            data.controllengthe = String.Empty : controllengthe = 0

            data.fabrictypef = data.fabrictyped : data.fabriccolourf = data.fabriccolourd
            data.rollf = data.rolld

            data.printingb = String.Empty
            data.printingc = String.Empty
            data.printingd = String.Empty
            data.printinge = String.Empty
            data.printingf = String.Empty
            data.printingg = String.Empty
            data.printingh = String.Empty

            If width > 1510 OrElse drop > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printing)
                data.printing = String.Empty
            End If

            If widthb > 1510 OrElse dropb > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingb)
                data.printingb = String.Empty
            End If

            If widthc > 1510 OrElse dropc > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingc)
                data.printingc = String.Empty
            End If

            If widthd > 1510 OrElse dropd > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingd)
                data.printingd = String.Empty
            End If

            If widthe > 1510 OrElse drope > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printinge)
                data.printinge = String.Empty
            End If

            If widthf > 1510 OrElse dropf > 1510 Then
                orderClass.DeleteFilePrinting(data.orderid, data.printingf)
                data.printingf = String.Empty
            End If

            If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" OrElse controlName = "Sonesse 30 WF" Then
                data.chaincolour = data.remote : data.chaincolourc = String.Empty
                data.chaincolourd = String.Empty : data.chaincolourf = String.Empty
                data.chainstopper = String.Empty : data.chainstopperc = String.Empty
                data.chainstopperd = String.Empty : data.chainstopperf = String.Empty
                data.controllength = String.Empty : data.controllengthc = String.Empty
                data.controllengthd = String.Empty : data.controllengthf = String.Empty
                controllength = 0 : controllengthc = 0 : controllengthd = 0 : controllengthe = 0 : controllengthf = 0
                data.springassist = String.Empty
            End If

            If controlName = "Chain" Then
                If String.IsNullOrEmpty(data.chaincolourc) Then
                    data.chaincolourc = data.chaincolour
                    data.chainstopperc = data.chainstopper
                    data.controllengthc = data.controllength
                    chainTypeC = orderClass.GetChainType(data.chaincolourc)
                End If
                If String.IsNullOrEmpty(data.chainstopperc) Then
                    data.chainstopperc = data.chainstopper
                End If

                If String.IsNullOrEmpty(data.chaincolourd) Then
                    data.chaincolourd = data.chaincolour
                    data.chainstopperd = data.chainstopper
                    data.controllengthd = data.controllength
                    chainTypeD = orderClass.GetChainType(data.chaincolourd)
                End If
                If String.IsNullOrEmpty(data.chainstopperd) Then
                    data.chainstopperd = data.chainstopper
                End If

                If String.IsNullOrEmpty(data.chaincolourf) Then
                    data.chaincolourf = data.chaincolour
                    data.chainstopperf = data.chainstopper
                    data.controllengthf = data.controllength
                    chainTypeF = orderClass.GetChainType(data.chaincolourf)
                End If
                If String.IsNullOrEmpty(data.chainstopperd) Then
                    data.chainstopperd = data.chainstopper
                End If

                If data.controllength = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(drop * 2 / 3)
                    If chainType = "Non Continuous" Then
                        controllength = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllength = Math.Ceiling((drop * 3 / 4) + 80)
                        End If
                    End If
                    If chainType = "Continuous" Then
                        controllength = 500
                        If stdControlLength > 500 Then controllength = 750
                        If stdControlLength > 750 Then controllength = 1000
                        If stdControlLength > 1000 Then controllength = 1200
                        If stdControlLength > 1200 Then controllength = 1500
                    End If
                End If

                If data.controllengthc = "" OrElse data.controllengthc = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropc * 2 / 3)
                    If chainTypeC = "Non Continuous" Then
                        controllengthc = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthc = Math.Ceiling((dropc * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeC = "Continuous" Then
                        controllengthc = 500
                        If stdControlLength > 500 Then controllengthc = 750
                        If stdControlLength > 750 Then controllengthc = 1000
                        If stdControlLength > 1000 Then controllengthc = 1200
                        If stdControlLength > 1200 Then controllengthc = 1500
                    End If

                    If Not tubeName = "Acmeda 49mm" Then data.springassist = String.Empty
                End If

                If data.controllengthd = "" OrElse data.controllengthd = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropd * 2 / 3)
                    If chainTypeD = "Non Continuous" Then
                        controllengthd = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthd = Math.Ceiling((dropd * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeD = "Continuous" Then
                        controllengthd = 500
                        If stdControlLength > 500 Then controllengthd = 750
                        If stdControlLength > 750 Then controllengthd = 1000
                        If stdControlLength > 1000 Then controllengthd = 1200
                        If stdControlLength > 1200 Then controllengthd = 1500
                    End If
                End If

                If data.controllengthf = "" OrElse data.controllengthf = "Standard" Then
                    Dim stdControlLength As Integer = Math.Ceiling(dropf * 2 / 3)
                    If chainTypeF = "Non Continuous" Then
                        controllengthf = stdControlLength
                        If tubeName.Contains("Gear Reduction") Then
                            controllengthf = Math.Ceiling((dropf * 3 / 4) + 80)
                        End If
                    End If
                    If chainTypeF = "Continuous" Then
                        controllengthf = 500
                        If stdControlLength > 500 Then controllengthf = 750
                        If stdControlLength > 750 Then controllengthf = 1000
                        If stdControlLength > 1000 Then controllengthf = 1200
                        If stdControlLength > 1200 Then controllengthf = 1500
                    End If
                End If

                If controlName = "Altus" OrElse controlName = "Mercure" OrElse controlName = "LSN40" Then
                    data.charger = String.Empty
                End If

                If Not controlName.Contains("Alpha") Then
                    data.extensioncable = String.Empty
                    data.supply = String.Empty
                End If

                If data.companyid = "2" Then data.drycontact = String.Empty
                If controlName = "Alpha 1Nm WF" OrElse controlName = "Alpha 2Nm WF" OrElse controlName = "Alpha 3Nm WF" OrElse controlName = "Alpha 5Nm HW" OrElse controlName = "Mercure" OrElse controlName = "Chain" Then
                    data.drycontact = String.Empty
                End If

                If String.IsNullOrEmpty(data.bottomtypeb) Then
                    data.bottomtypeb = data.bottomtype
                    data.bottomcolourb = data.bottomcolour
                    data.bottomoptionb = data.bottomoption
                    bottomNameB = orderClass.GetBottomName(data.bottomtypeb)
                End If

                If String.IsNullOrEmpty(data.bottomtypec) Then
                    data.bottomtypec = data.bottomtype
                    data.bottomcolourc = data.bottomcolour
                    data.bottomoptionc = data.bottomoption
                    bottomNameC = orderClass.GetBottomName(data.bottomtypec)
                End If

                If String.IsNullOrEmpty(data.bottomtyped) Then
                    data.bottomtyped = data.bottomtype
                    data.bottomcolourd = data.bottomcolour
                    data.bottomoptiond = data.bottomoption
                    bottomNameD = orderClass.GetBottomName(data.bottomtyped)
                End If

                If String.IsNullOrEmpty(data.bottomtypee) Then
                    data.bottomtypee = data.bottomtype
                    data.bottomcoloure = data.bottomcolour
                    data.bottomoptione = data.bottomoption
                    bottomNameE = orderClass.GetBottomName(data.bottomtypee)
                End If

                If String.IsNullOrEmpty(data.bottomtypef) Then
                    data.bottomtypef = data.bottomtype
                    data.bottomcolourf = data.bottomcolour
                    data.bottomoptionf = data.bottomoption
                    bottomNameF = orderClass.GetBottomName(data.bottomtypef)
                End If

                If Not (bottomName = "Flat" OrElse bottomName = "Flat Mohair") Then
                    data.bottomoption = ""
                End If
                If Not (bottomNameB = "Flat" OrElse bottomNameB = "Flat Mohair") Then
                    data.bottomoptionb = ""
                End If
                If Not (bottomNameC = "Flat" OrElse bottomNameC = "Flat Mohair") Then
                    data.bottomoptionc = ""
                End If
                If Not (bottomNameD = "Flat" OrElse bottomNameD = "Flat Mohair") Then
                    data.bottomoptiond = ""
                End If
                If Not (bottomNameE = "Flat" OrElse bottomNameE = "Flat Mohair") Then
                    data.bottomoptione = ""
                End If
                If Not (bottomNameF = "Flat" OrElse bottomNameF = "Flat Mohair") Then
                    data.bottomoptionf = ""
                End If

                If bottomName = "Flat Mohair" Then data.bottomoption = "Fabric on Front"
                If bottomNameB = "Flat Mohair" Then data.bottomoptionb = "Fabric on Front"
                If bottomNameC = "Flat Mohair" Then data.bottomoptionc = "Fabric on Front"
                If bottomNameD = "Flat Mohair" Then data.bottomoptiond = "Fabric on Front"
                If bottomNameE = "Flat Mohair" Then data.bottomoptione = "Fabric on Front"
                If bottomNameF = "Flat Mohair" Then data.bottomoptionf = "Fabric on Front"

                If tubeName.Contains("Gear Reduction") OrElse tubeName.Contains("Acmeda") OrElse tubeName = "Standard" Then
                    data.adjusting = String.Empty
                End If
            End If

            linearMetre = width / 1000
            linearMetreB = widthb / 1000
            linearMetreC = widthc / 1000
            linearMetreC = widthd / 1000
            linearMetreE = widthe / 1000
            linearMetreF = widthf / 1000

            squareMetre = width * drop / 1000000
            squareMetreB = widthb * dropb / 1000000
            squareMetreC = widthc * dropc / 1000000
            squareMetreD = widthd * dropd / 1000000
            squareMetreE = widthe * drope / 1000000
            squareMetreF = widthf * dropf / 1000000

            totalItems = 6

            groupFabric = orderClass.GetFabricGroup(data.fabrictype)
            groupFabricDB = orderClass.GetFabricGroup(data.fabrictyped)

            Dim tubeIstilah As String = "Standard"
            If tubeName.Contains("Gear Reduction") Then tubeIstilah = "Gear Reduction"
            If tubeName.Contains("Sunboss") Then tubeIstilah = "Sunboss"
            If tubeName.Contains("Acmeda") Then tubeIstilah = "Acmeda"

            If data.companyid = "3" Then
                tubeIstilah = tubeName
                If data.customerid = "902" OrElse data.customerid = "953" Then
                    tubeIstilah = "Gear Reduction 38mm"
                End If
                groupFabric = orderClass.GetFabricGroupLocal("Roller", data.fabrictype)
                groupFabricDB = orderClass.GetFabricGroupLocal("Roller", data.fabrictyped)
            End If

            Dim groupName As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabric)
            Dim groupNameDB As String = String.Format("{0} - {1} - {2}", designName, tubeIstilah, groupFabricDB)

            priceProductGroup = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupC = orderClass.GetPriceProductGroupId(groupName, data.designid)
            priceProductGroupD = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
            priceProductGroupE = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
            priceProductGroupF = orderClass.GetPriceProductGroupId(groupNameDB, data.designid)
        End If

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricIdB, FabricIdC, FabricIdD, FabricIdE, FabricIdF, FabricColourId, FabricColourIdB, FabricColourIdC, FabricColourIdD, FabricColourIdE, FabricColourIdF, ChainId, ChainIdB, ChainIdC, ChainIdD, ChainIdE, ChainIdF, BottomId, BottomIdB, BottomIdC, BottomIdD, BottomIdE, BottomIdF, BottomColourId, BottomColourIdB, BottomColourIdC, BottomColourIdD, BottomColourIdE, BottomColourIdF, PriceProductGroupId, PriceProductGroupIdB, PriceProductGroupIdC, PriceProductGroupIdD, PriceProductGroupIdE, PriceProductGroupIdF, Qty, Room, Mounting, Width, WidthB, WidthC, WidthD, WidthE, WidthF, [Drop], DropB, DropC, DropD, DropE, DropF, Roll, RollB, RollC, RollD, RollE, RollF, ControlPosition, ControlPositionB, ControlPositionC, ControlPositionD, ControlPositionE, ControlPositionF, ControlLength, ControlLengthB, ControlLengthC, ControlLengthD, ControlLengthE, ControlLengthF, ControlLengthValue, ControlLengthValueB, ControlLengthValueC, ControlLengthValueD, ControlLengthValueE, ControlLengthValueF, ChainStopper, ChainStopperB, ChainStopperC, ChainStopperD, ChainStopperE, ChainStopperF, FlatOption, FlatOptionB, FlatOptionC, FlatOptionD, FlatOptionE, FlatOptionF, TopTrack, BracketSize, BracketExtension, SpringAssist, Adjusting, DryContact, Charger, ExtensionCable, Supply, LinearMetre, LinearMetreB, LinearMetreC, LinearMetreD, LinearMetreE, LinearMetreF, SquareMetre, SquareMetreB, SquareMetreC, SquareMetreD, SquareMetreE, SquareMetreF, Printing, PrintingB, PrintingC, PrintingD, PrintingE, PrintingF, PrintingG, PrintingH, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricIdB, @FabricIdC, @FabricIdD, @FabricIdE, @FabricIdF, @FabricColourId, @FabricColourIdB, @FabricColourIdC, @FabricColourIdD, @FabricColourIdE, @FabricColourIdF, @ChainId, @ChainIdB, @ChainIdC, @ChainIdD, @ChainIdE, @ChainIdF, @BottomId, @BottomIdB, @BottomIdC, @BottomIdD, @BottomIdE, @BottomIdF, @BottomColourId, @BottomColourIdB, @BottomColourIdC, @BottomColourIdD, @BottomColourIdE, @BottomColourIdF, @PriceProductGroupId, @PriceProductGroupIdB, @PriceProductGroupIdC, @PriceProductGroupIdD, @PriceProductGroupIdE, @PriceProductGroupIdF, @Qty, @Room, @Mounting, @Width, @WidthB, @WidthC, @WidthD, @WidthE, @WidthF, @Drop, @DropB, @DropC, @DropD, @DropE, @DropF, @Roll, @RollB, @RollC, @RollD, @RollE, @RollF, @ControlPosition, @ControlPositionB, @ControlPositionC, @ControlPositionD, @ControlPositionE, @ControlPositionF, @ControlLength, @ControlLengthB, @ControlLengthC, @ControlLengthD, @ControlLengthE, @ControlLengthF, @ControlLengthValue, @ControlLengthValueB, @ControlLengthValueC, @ControlLengthValueD, @ControlLengthValueE, @ControlLengthValueF, @ChainStopper, @ChainStopperB, @ChainStopperC, @ChainStopperD, @ChainStopperE, @ChainStopperF, @FlatOption, @FlatOptionB, @FlatOptionC, @FlatOptionD, @FlatOptionE, @FlatOptionF, @TopTrack, @BracketSize, @BracketExtension, @SpringAssist, @Adjusting, @DryContact, @Charger, @ExtensionCable, @Supply, @LinearMetre, @LinearMetreB, @LinearMetreC, @LinearMetreD, @LinearMetreE, @LinearMetreF, @SquareMetre, @SquareMetreB, @SquareMetreC, @SquareMetreD, @SquareMetreE, @SquareMetreF, @Printing, @PrintingB, @PrintingC, @PrintingD, @PrintingE, @PrintingF, @PrintingG, @PrintingH, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabrictypeb), CType(DBNull.Value, Object), data.fabrictypeb))
                        myCmd.Parameters.AddWithValue("@FabricIdC", If(String.IsNullOrEmpty(data.fabrictypec), CType(DBNull.Value, Object), data.fabrictypec))
                        myCmd.Parameters.AddWithValue("@FabricIdD", If(String.IsNullOrEmpty(data.fabrictyped), CType(DBNull.Value, Object), data.fabrictyped))
                        myCmd.Parameters.AddWithValue("@FabricIdE", If(String.IsNullOrEmpty(data.fabrictypee), CType(DBNull.Value, Object), data.fabrictypee))
                        myCmd.Parameters.AddWithValue("@FabricIdF", If(String.IsNullOrEmpty(data.fabrictypef), CType(DBNull.Value, Object), data.fabrictypef))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(data.fabriccolourb), CType(DBNull.Value, Object), data.fabriccolourb))
                        myCmd.Parameters.AddWithValue("@FabricColourIdC", If(String.IsNullOrEmpty(data.fabriccolourc), CType(DBNull.Value, Object), data.fabriccolourc))
                        myCmd.Parameters.AddWithValue("@FabricColourIdD", If(String.IsNullOrEmpty(data.fabriccolourd), CType(DBNull.Value, Object), data.fabriccolourd))
                        myCmd.Parameters.AddWithValue("@FabricColourIdE", If(String.IsNullOrEmpty(data.fabriccoloure), CType(DBNull.Value, Object), data.fabriccoloure))
                        myCmd.Parameters.AddWithValue("@FabricColourIdF", If(String.IsNullOrEmpty(data.fabriccolourf), CType(DBNull.Value, Object), data.fabriccolourf))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                        myCmd.Parameters.AddWithValue("@ChainIdB", If(String.IsNullOrEmpty(data.chaincolourb), CType(DBNull.Value, Object), data.chaincolourb))
                        myCmd.Parameters.AddWithValue("@ChainIdC", If(String.IsNullOrEmpty(data.chaincolourc), CType(DBNull.Value, Object), data.chaincolourc))
                        myCmd.Parameters.AddWithValue("@ChainIdD", If(String.IsNullOrEmpty(data.chaincolourd), CType(DBNull.Value, Object), data.chaincolourd))
                        myCmd.Parameters.AddWithValue("@ChainIdE", If(String.IsNullOrEmpty(data.chaincoloure), CType(DBNull.Value, Object), data.chaincoloure))
                        myCmd.Parameters.AddWithValue("@ChainIdF", If(String.IsNullOrEmpty(data.chaincolourf), CType(DBNull.Value, Object), data.chaincolourf))
                        myCmd.Parameters.AddWithValue("@BottomId", If(String.IsNullOrEmpty(data.bottomtype), CType(DBNull.Value, Object), data.bottomtype))
                        myCmd.Parameters.AddWithValue("@BottomIdB", If(String.IsNullOrEmpty(data.bottomtypeb), CType(DBNull.Value, Object), data.bottomtypeb))
                        myCmd.Parameters.AddWithValue("@BottomIdC", If(String.IsNullOrEmpty(data.bottomtypec), CType(DBNull.Value, Object), data.bottomtypec))
                        myCmd.Parameters.AddWithValue("@BottomIdD", If(String.IsNullOrEmpty(data.bottomtyped), CType(DBNull.Value, Object), data.bottomtyped))
                        myCmd.Parameters.AddWithValue("@BottomIdE", If(String.IsNullOrEmpty(data.bottomtypee), CType(DBNull.Value, Object), data.bottomtypee))
                        myCmd.Parameters.AddWithValue("@BottomIdF", If(String.IsNullOrEmpty(data.bottomtypef), CType(DBNull.Value, Object), data.bottomtypef))
                        myCmd.Parameters.AddWithValue("@BottomColourId", If(String.IsNullOrEmpty(data.bottomcolour), CType(DBNull.Value, Object), data.bottomcolour))
                        myCmd.Parameters.AddWithValue("@BottomColourIdB", If(String.IsNullOrEmpty(data.bottomcolourb), CType(DBNull.Value, Object), data.bottomcolourb))
                        myCmd.Parameters.AddWithValue("@BottomColourIdC", If(String.IsNullOrEmpty(data.bottomcolourc), CType(DBNull.Value, Object), data.bottomcolourc))
                        myCmd.Parameters.AddWithValue("@BottomColourIdD", If(String.IsNullOrEmpty(data.bottomcolourd), CType(DBNull.Value, Object), data.bottomcolourd))
                        myCmd.Parameters.AddWithValue("@BottomColourIdE", If(String.IsNullOrEmpty(data.bottomcoloure), CType(DBNull.Value, Object), data.bottomcoloure))
                        myCmd.Parameters.AddWithValue("@BottomColourIdF", If(String.IsNullOrEmpty(data.bottomcolourf), CType(DBNull.Value, Object), data.bottomcolourf))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdC", If(String.IsNullOrEmpty(priceProductGroupC), CType(DBNull.Value, Object), priceProductGroupC))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdD", If(String.IsNullOrEmpty(priceProductGroupD), CType(DBNull.Value, Object), priceProductGroupD))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdE", If(String.IsNullOrEmpty(priceProductGroupE), CType(DBNull.Value, Object), priceProductGroupE))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdF", If(String.IsNullOrEmpty(priceProductGroupF), CType(DBNull.Value, Object), priceProductGroupF))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@WidthB", widthb)
                        myCmd.Parameters.AddWithValue("@WidthC", widthc)
                        myCmd.Parameters.AddWithValue("@WidthD", widthd)
                        myCmd.Parameters.AddWithValue("@WidthE", widthe)
                        myCmd.Parameters.AddWithValue("@WidthF", widthf)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@DropB", dropb)
                        myCmd.Parameters.AddWithValue("@DropC", dropc)
                        myCmd.Parameters.AddWithValue("@DropD", dropd)
                        myCmd.Parameters.AddWithValue("@DropE", drope)
                        myCmd.Parameters.AddWithValue("@DropF", dropf)
                        myCmd.Parameters.AddWithValue("@Roll", data.roll)
                        myCmd.Parameters.AddWithValue("@RollB", data.rollb)
                        myCmd.Parameters.AddWithValue("@RollC", data.rollc)
                        myCmd.Parameters.AddWithValue("@RollD", data.rolld)
                        myCmd.Parameters.AddWithValue("@RollE", data.rolle)
                        myCmd.Parameters.AddWithValue("@RollF", data.rollf)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ControlPositionB", data.controlpositionb)
                        myCmd.Parameters.AddWithValue("@ControlPositionC", data.controlpositionc)
                        myCmd.Parameters.AddWithValue("@ControlPositionD", data.controlpositiond)
                        myCmd.Parameters.AddWithValue("@ControlPositionE", data.controlpositione)
                        myCmd.Parameters.AddWithValue("@ControlPositionF", data.controlpositionf)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthB", data.controllengthb)
                        myCmd.Parameters.AddWithValue("@ControlLengthC", data.controllengthc)
                        myCmd.Parameters.AddWithValue("@ControlLengthD", data.controllengthd)
                        myCmd.Parameters.AddWithValue("@ControlLengthE", data.controllengthe)
                        myCmd.Parameters.AddWithValue("@ControlLengthF", data.controllengthf)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValueB", controllengthb)
                        myCmd.Parameters.AddWithValue("@ControlLengthValueC", controllengthc)
                        myCmd.Parameters.AddWithValue("@ControlLengthValueD", controllengthd)
                        myCmd.Parameters.AddWithValue("@ControlLengthValueE", controllengthe)
                        myCmd.Parameters.AddWithValue("@ControlLengthValueF", controllengthf)
                        myCmd.Parameters.AddWithValue("@ChainStopper", data.chainstopper)
                        myCmd.Parameters.AddWithValue("@ChainStopperB", data.chainstopperb)
                        myCmd.Parameters.AddWithValue("@ChainStopperC", data.chainstopperc)
                        myCmd.Parameters.AddWithValue("@ChainStopperD", data.chainstopperd)
                        myCmd.Parameters.AddWithValue("@ChainStopperE", data.chainstoppere)
                        myCmd.Parameters.AddWithValue("@ChainStopperF", data.chainstopperf)
                        myCmd.Parameters.AddWithValue("@FlatOption", data.bottomoption)
                        myCmd.Parameters.AddWithValue("@FlatOptionB", data.bottomoptionb)
                        myCmd.Parameters.AddWithValue("@FlatOptionC", data.bottomoptionc)
                        myCmd.Parameters.AddWithValue("@FlatOptionD", data.bottomoptiond)
                        myCmd.Parameters.AddWithValue("@FlatOptionE", data.bottomoptione)
                        myCmd.Parameters.AddWithValue("@FlatOptionF", data.bottomoptionf)
                        myCmd.Parameters.AddWithValue("@TopTrack", data.toptrack)
                        myCmd.Parameters.AddWithValue("@BracketSize", If(String.IsNullOrEmpty(data.bracketsize), CType(DBNull.Value, Object), data.bracketsize))
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketextension)
                        myCmd.Parameters.AddWithValue("@SpringAssist", data.springassist)
                        myCmd.Parameters.AddWithValue("@Adjusting", data.adjusting)
                        myCmd.Parameters.AddWithValue("@DryContact", data.drycontact)
                        myCmd.Parameters.AddWithValue("@Charger", data.charger)
                        myCmd.Parameters.AddWithValue("@ExtensionCable", data.extensioncable)
                        myCmd.Parameters.AddWithValue("@Supply", data.supply)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                        myCmd.Parameters.AddWithValue("@LinearMetreC", linearMetreC)
                        myCmd.Parameters.AddWithValue("@LinearMetreD", linearMetreD)
                        myCmd.Parameters.AddWithValue("@LinearMetreE", linearMetreE)
                        myCmd.Parameters.AddWithValue("@LinearMetreF", linearMetreF)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                        myCmd.Parameters.AddWithValue("@SquareMetreC", squareMetreC)
                        myCmd.Parameters.AddWithValue("@SquareMetreD", squareMetreD)
                        myCmd.Parameters.AddWithValue("@SquareMetreE", squareMetreE)
                        myCmd.Parameters.AddWithValue("@SquareMetreF", squareMetreF)
                        myCmd.Parameters.AddWithValue("@Printing", data.printing)
                        myCmd.Parameters.AddWithValue("@PrintingB", data.printingb)
                        myCmd.Parameters.AddWithValue("@PrintingC", data.printingc)
                        myCmd.Parameters.AddWithValue("@PrintingD", data.printingd)
                        myCmd.Parameters.AddWithValue("@PrintingE", data.printinge)
                        myCmd.Parameters.AddWithValue("@PrintingF", data.printingf)
                        myCmd.Parameters.AddWithValue("@PrintingG", data.printingg)
                        myCmd.Parameters.AddWithValue("@PrintingH", data.printingh)
                        myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next

            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricIdB=@FabricIdB, FabricIdC=@FabricIdC, FabricIdD=@FabricIdD, FabricIdE=@FabricIdE, FabricIdF=@FabricIdF, FabricColourId=@FabricColourId, FabricColourIdB=@FabricColourIdB, FabricColourIdC=@FabricColourIdC, FabricColourIdD=@FabricColourIdD, FabricColourIdE=@FabricColourIdE, FabricColourIdF=@FabricColourIdF, ChainId=@ChainId, ChainIdB=@ChainIdB, ChainIdC=@ChainIdC, ChainIdD=@ChainIdD, ChainIdE=@ChainIdE, ChainIdF=@ChainIdF, BottomId=@BottomId, BottomIdB=@BottomIdB, BottomIdC=@BottomIdC, BottomIdD=@BottomIdD, BottomIdE=@BottomIdE, BottomIdF=@BottomIdF, BottomColourId=@BottomColourId, BottomColourIdB=@BottomColourIdB, BottomColourIdC=@BottomColourIdC, BottomColourIdD=@BottomColourIdD, BottomColourIdE=@BottomColourIdE, BottomColourIdF=@BottomColourIdF, PriceProductGroupId=@PriceProductGroupId, PriceProductGroupIdB=@PriceProductGroupIdB, PriceProductGroupIdC=@PriceProductGroupIdC, PriceProductGroupIdD=@PriceProductGroupIdD, PriceProductGroupIdE=@PriceProductGroupIdE, PriceProductGroupIdF=@PriceProductGroupIdF, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, WidthB=@WidthB, WidthC=@WidthC, WidthD=@WidthD, WidthE=@WidthE, WidthF=@WidthF, [Drop]=@Drop, DropB=@DropB, DropC=@DropC, DropD=@DropD, DropE=@DropE, DropF=@DropF, Roll=@Roll, RollB=@RollB, RollC=@RollC, RollD=@RollD, RollE=@RollE, RollF=@RollF, ControlPosition=@ControlPosition, ControlPositionB=@ControlPositionB, ControlPositionC=@ControlPositionC, ControlPositionD=@ControlPositionD, ControlPositionE=@ControlPositionE, ControlPositionF=@ControlPositionF, ControlLength=@ControlLength, ControlLengthB=@ControlLengthB, ControlLengthC=@ControlLengthC, ControlLengthD=@ControlLengthD, ControlLengthE=@ControlLengthE, ControlLengthF=@ControlLengthF, ControlLengthValue=@ControlLengthValue, ControlLengthValueB=@ControlLengthValueB, ControlLengthValueC=@ControlLengthValueC, ControlLengthValueD=@ControlLengthValueD, ControlLengthValueE=@ControlLengthValueE, ControlLengthValueF=@ControlLengthValueF, ChainStopper=@ChainStopper, ChainStopperB=@ChainStopperB, ChainStopperC=@ChainStopperC, ChainStopperD=@ChainStopperD, ChainStopperE=@ChainStopperE, ChainStopperF=@ChainStopperF, FlatOption=@FlatOption, FlatOptionB=@FlatOptionB, FlatOptionC=@FlatOptionC, FlatOptionD=@FlatOptionD, FlatOptionE=@FlatOptionE, FlatOptionF=@FlatOptionF, TopTrack=@TopTrack, BracketSize=@BracketSize, BracketExtension=@BracketExtension, SpringAssist=@SpringAssist, Adjusting=@Adjusting, DryContact=@DryContact, Charger=@Charger, ExtensionCable=@ExtensionCable, Supply=@Supply, LinearMetre=@LinearMetre, LinearMetreB=@LinearMetreB, LinearMetreC=@LinearMetreC, LinearMetreD=@LinearMetreD, LinearMetreE=@LinearMetreE, LinearMetreF=@LinearMetreF, SquareMetre=@SquareMetre, SquareMetreB=@SquareMetreB, SquareMetreC=@SquareMetreC, SquareMetreD=@SquareMetreD, SquareMetreE=@SquareMetreE, SquareMetreF=@SquareMetreF, Printing=@Printing, PrintingB=@PrintingB, PrintingC=@PrintingC, PrintingD=@PrintingD, PrintingE=@PrintingE, PrintingF=@PrintingF, PrintingG=@PrintingG, PrintingH=@PrintingH, TotalItems=@TotalItems, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricIdB", If(String.IsNullOrEmpty(data.fabrictypeb), CType(DBNull.Value, Object), data.fabrictypeb))
                    myCmd.Parameters.AddWithValue("@FabricIdC", If(String.IsNullOrEmpty(data.fabrictypec), CType(DBNull.Value, Object), data.fabrictypec))
                    myCmd.Parameters.AddWithValue("@FabricIdD", If(String.IsNullOrEmpty(data.fabrictyped), CType(DBNull.Value, Object), data.fabrictyped))
                    myCmd.Parameters.AddWithValue("@FabricIdE", If(String.IsNullOrEmpty(data.fabrictypee), CType(DBNull.Value, Object), data.fabrictypee))
                    myCmd.Parameters.AddWithValue("@FabricIdF", If(String.IsNullOrEmpty(data.fabrictypef), CType(DBNull.Value, Object), data.fabrictypef))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@FabricColourIdB", If(String.IsNullOrEmpty(data.fabriccolourb), CType(DBNull.Value, Object), data.fabriccolourb))
                    myCmd.Parameters.AddWithValue("@FabricColourIdC", If(String.IsNullOrEmpty(data.fabriccolourc), CType(DBNull.Value, Object), data.fabriccolourc))
                    myCmd.Parameters.AddWithValue("@FabricColourIdD", If(String.IsNullOrEmpty(data.fabriccolourd), CType(DBNull.Value, Object), data.fabriccolourd))
                    myCmd.Parameters.AddWithValue("@FabricColourIdE", If(String.IsNullOrEmpty(data.fabriccoloure), CType(DBNull.Value, Object), data.fabriccoloure))
                    myCmd.Parameters.AddWithValue("@FabricColourIdF", If(String.IsNullOrEmpty(data.fabriccolourf), CType(DBNull.Value, Object), data.fabriccolourf))
                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                    myCmd.Parameters.AddWithValue("@ChainIdB", If(String.IsNullOrEmpty(data.chaincolourb), CType(DBNull.Value, Object), data.chaincolourb))
                    myCmd.Parameters.AddWithValue("@ChainIdC", If(String.IsNullOrEmpty(data.chaincolourc), CType(DBNull.Value, Object), data.chaincolourc))
                    myCmd.Parameters.AddWithValue("@ChainIdD", If(String.IsNullOrEmpty(data.chaincolourd), CType(DBNull.Value, Object), data.chaincolourd))
                    myCmd.Parameters.AddWithValue("@ChainIdE", If(String.IsNullOrEmpty(data.chaincoloure), CType(DBNull.Value, Object), data.chaincoloure))
                    myCmd.Parameters.AddWithValue("@ChainIdF", If(String.IsNullOrEmpty(data.chaincolourf), CType(DBNull.Value, Object), data.chaincolourf))
                    myCmd.Parameters.AddWithValue("@BottomId", If(String.IsNullOrEmpty(data.bottomtype), CType(DBNull.Value, Object), data.bottomtype))
                    myCmd.Parameters.AddWithValue("@BottomIdB", If(String.IsNullOrEmpty(data.bottomtypeb), CType(DBNull.Value, Object), data.bottomtypeb))
                    myCmd.Parameters.AddWithValue("@BottomIdC", If(String.IsNullOrEmpty(data.bottomtypec), CType(DBNull.Value, Object), data.bottomtypec))
                    myCmd.Parameters.AddWithValue("@BottomIdD", If(String.IsNullOrEmpty(data.bottomtyped), CType(DBNull.Value, Object), data.bottomtyped))
                    myCmd.Parameters.AddWithValue("@BottomIdE", If(String.IsNullOrEmpty(data.bottomtypee), CType(DBNull.Value, Object), data.bottomtypee))
                    myCmd.Parameters.AddWithValue("@BottomIdF", If(String.IsNullOrEmpty(data.bottomtypef), CType(DBNull.Value, Object), data.bottomtypef))
                    myCmd.Parameters.AddWithValue("@BottomColourId", If(String.IsNullOrEmpty(data.bottomcolour), CType(DBNull.Value, Object), data.bottomcolour))
                    myCmd.Parameters.AddWithValue("@BottomColourIdB", If(String.IsNullOrEmpty(data.bottomcolourb), CType(DBNull.Value, Object), data.bottomcolourb))
                    myCmd.Parameters.AddWithValue("@BottomColourIdC", If(String.IsNullOrEmpty(data.bottomcolourc), CType(DBNull.Value, Object), data.bottomcolourc))
                    myCmd.Parameters.AddWithValue("@BottomColourIdD", If(String.IsNullOrEmpty(data.bottomcolourd), CType(DBNull.Value, Object), data.bottomcolourd))
                    myCmd.Parameters.AddWithValue("@BottomColourIdE", If(String.IsNullOrEmpty(data.bottomcoloure), CType(DBNull.Value, Object), data.bottomcoloure))
                    myCmd.Parameters.AddWithValue("@BottomColourIdF", If(String.IsNullOrEmpty(data.bottomcolourf), CType(DBNull.Value, Object), data.bottomcolourf))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdC", If(String.IsNullOrEmpty(priceProductGroupC), CType(DBNull.Value, Object), priceProductGroupC))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdD", If(String.IsNullOrEmpty(priceProductGroupD), CType(DBNull.Value, Object), priceProductGroupD))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdE", If(String.IsNullOrEmpty(priceProductGroupE), CType(DBNull.Value, Object), priceProductGroupE))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdF", If(String.IsNullOrEmpty(priceProductGroupF), CType(DBNull.Value, Object), priceProductGroupF))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@WidthB", widthb)
                    myCmd.Parameters.AddWithValue("@WidthC", widthc)
                    myCmd.Parameters.AddWithValue("@WidthD", widthd)
                    myCmd.Parameters.AddWithValue("@WidthE", widthe)
                    myCmd.Parameters.AddWithValue("@WidthF", widthf)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@DropB", dropb)
                    myCmd.Parameters.AddWithValue("@DropC", dropc)
                    myCmd.Parameters.AddWithValue("@DropD", dropd)
                    myCmd.Parameters.AddWithValue("@DropE", drope)
                    myCmd.Parameters.AddWithValue("@DropF", dropf)
                    myCmd.Parameters.AddWithValue("@Roll", data.roll)
                    myCmd.Parameters.AddWithValue("@RollB", data.rollb)
                    myCmd.Parameters.AddWithValue("@RollC", data.rollc)
                    myCmd.Parameters.AddWithValue("@RollD", data.rolld)
                    myCmd.Parameters.AddWithValue("@RollE", data.rolle)
                    myCmd.Parameters.AddWithValue("@RollF", data.rollf)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@ControlPositionB", data.controlpositionb)
                    myCmd.Parameters.AddWithValue("@ControlPositionC", data.controlpositionc)
                    myCmd.Parameters.AddWithValue("@ControlPositionD", data.controlpositiond)
                    myCmd.Parameters.AddWithValue("@ControlPositionE", data.controlpositione)
                    myCmd.Parameters.AddWithValue("@ControlPositionF", data.controlpositionf)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthB", data.controllengthb)
                    myCmd.Parameters.AddWithValue("@ControlLengthC", data.controllengthc)
                    myCmd.Parameters.AddWithValue("@ControlLengthD", data.controllengthd)
                    myCmd.Parameters.AddWithValue("@ControlLengthE", data.controllengthe)
                    myCmd.Parameters.AddWithValue("@ControlLengthF", data.controllengthf)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValueB", controllengthb)
                    myCmd.Parameters.AddWithValue("@ControlLengthValueC", controllengthc)
                    myCmd.Parameters.AddWithValue("@ControlLengthValueD", controllengthd)
                    myCmd.Parameters.AddWithValue("@ControlLengthValueE", controllengthe)
                    myCmd.Parameters.AddWithValue("@ControlLengthValueF", controllengthf)
                    myCmd.Parameters.AddWithValue("@ChainStopper", data.chainstopper)
                    myCmd.Parameters.AddWithValue("@ChainStopperB", data.chainstopperb)
                    myCmd.Parameters.AddWithValue("@ChainStopperC", data.chainstopperc)
                    myCmd.Parameters.AddWithValue("@ChainStopperD", data.chainstopperd)
                    myCmd.Parameters.AddWithValue("@ChainStopperE", data.chainstoppere)
                    myCmd.Parameters.AddWithValue("@ChainStopperF", data.chainstopperf)
                    myCmd.Parameters.AddWithValue("@FlatOption", data.bottomoption)
                    myCmd.Parameters.AddWithValue("@FlatOptionB", data.bottomoptionb)
                    myCmd.Parameters.AddWithValue("@FlatOptionC", data.bottomoptionc)
                    myCmd.Parameters.AddWithValue("@FlatOptionD", data.bottomoptiond)
                    myCmd.Parameters.AddWithValue("@FlatOptionE", data.bottomoptione)
                    myCmd.Parameters.AddWithValue("@FlatOptionF", data.bottomoptionf)
                    myCmd.Parameters.AddWithValue("@TopTrack", data.toptrack)
                    myCmd.Parameters.AddWithValue("@BracketSize", If(String.IsNullOrEmpty(data.bracketsize), CType(DBNull.Value, Object), data.bracketsize))
                    myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketextension)
                    myCmd.Parameters.AddWithValue("@SpringAssist", data.springassist)
                    myCmd.Parameters.AddWithValue("@Adjusting", data.adjusting)
                    myCmd.Parameters.AddWithValue("@DryContact", data.drycontact)
                    myCmd.Parameters.AddWithValue("@Charger", data.charger)
                    myCmd.Parameters.AddWithValue("@ExtensionCable", data.extensioncable)
                    myCmd.Parameters.AddWithValue("@Supply", data.supply)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@LinearMetreB", linearMetreB)
                    myCmd.Parameters.AddWithValue("@LinearMetreC", linearMetreC)
                    myCmd.Parameters.AddWithValue("@LinearMetreD", linearMetreD)
                    myCmd.Parameters.AddWithValue("@LinearMetreE", linearMetreE)
                    myCmd.Parameters.AddWithValue("@LinearMetreF", linearMetreF)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                    myCmd.Parameters.AddWithValue("@SquareMetreC", squareMetreC)
                    myCmd.Parameters.AddWithValue("@SquareMetreD", squareMetreD)
                    myCmd.Parameters.AddWithValue("@SquareMetreE", squareMetreE)
                    myCmd.Parameters.AddWithValue("@SquareMetreF", squareMetreF)
                    myCmd.Parameters.AddWithValue("@Printing", data.printing)
                    myCmd.Parameters.AddWithValue("@PrintingB", data.printingb)
                    myCmd.Parameters.AddWithValue("@PrintingC", data.printingc)
                    myCmd.Parameters.AddWithValue("@PrintingD", data.printingd)
                    myCmd.Parameters.AddWithValue("@PrintingE", data.printinge)
                    myCmd.Parameters.AddWithValue("@PrintingF", data.printingf)
                    myCmd.Parameters.AddWithValue("@PrintingG", data.printingg)
                    myCmd.Parameters.AddWithValue("@PrintingH", data.printingh)
                    myCmd.Parameters.AddWithValue("@TotalItems", totalItems)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function SkylineProccess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer = 0
        Dim drop As Integer = 0
        Dim midrailHeight1 As Integer = 0
        Dim midrailHeight2 As Integer = 0
        Dim headerLength As Integer = 0
        Dim horizontalHeight As Integer = 0

        Dim splitHeight1 As Integer = 0
        Dim splitHeight2 As Integer = 0

        Dim gap1 As Integer = 0
        Dim gap2 As Integer = 0
        Dim gap3 As Integer = 0
        Dim gap4 As Integer = 0
        Dim gap5 As Integer = 0

        Dim panelQty As Integer = 0
        Dim trackQty As Integer = 0
        Dim trackLength As Integer = 0
        Dim hingeQtyPerPanel As Integer = 0
        Dim panelQtyWithHinge As Integer = 0

        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = orderClass.GetBlindName(data.blindtype)
        Dim roleName As String = orderClass.GetUserRoleName(data.loginid)

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.designid) Then blindName = orderClass.GetBlindName(data.blindtype)

        If String.IsNullOrEmpty(data.blindtype) Then Return "SHUTTER TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "COLOUR IS REQUIRED !"


        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        If String.IsNullOrEmpty(data.drop) Then Return "HEIGHT IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR HEIGHT ORDER !"

        If String.IsNullOrEmpty(data.louvresize) Then Return "LOUVRE SIZE IS REQUIRED !"

        If blindName = "Track Sliding" Then
            If String.IsNullOrEmpty(data.louvreposition) Then Return "SLIDING LOUVRE POSITION IS REQUIRED !"
            If data.louvreposition = "Open" AndAlso data.louvresize = "114" Then Return "THE LOUVRE SIZE & LOUVRE POSITION YOU SELECT CANNOT BE PROCESSED !"
        End If

        If Not String.IsNullOrEmpty(data.midrailheight1) Then
            If Not Integer.TryParse(data.midrailheight1, midrailHeight1) OrElse midrailHeight1 < 0 Then Return "PLEASE CHECK YOUR MIDRAIL HEIGHT 1 ORDER !"
        End If
        If Not String.IsNullOrEmpty(data.midrailheight2) Then
            If Not Integer.TryParse(data.midrailheight2, midrailHeight2) OrElse midrailHeight2 < 0 Then Return "PLEASE CHECK YOUR MIDRAIL HEIGHT 2 ORDER !"
        End If
        If midrailHeight1 >= drop Then Return "THE HEIGHT OF MIDRAIL 1 SHOULD NOT BE EQUAL TO OR MORE THAN YOUR ORDER HEIGHT !"
        If midrailHeight2 >= drop Then Return "THE HEIGHT OF MIDRAIL 2 SHOULD NOT BE EQUAL TO OR MORE THAN YOUR ORDER HEIGHT !"
        If drop > 1500 Then
            If midrailHeight1 = 0 Then Return "MIDRAIL HEIGHT IS REQUIRED. <br /> MAXIMUM ONE SECTION IS 1500MM !"
        End If

        If roleName = "Customer" Then
            If midrailHeight1 > 0 AndAlso midrailHeight2 = 0 Then
                If midrailHeight1 > 1500 Then Return "MAXIMUM MIDRAIL HEIGHT 1 IS 1500MM !"
                If drop - midrailHeight1 > 1500 Then Return "MAXIMUM MIDRAIL HEIGHT FOR OTHER SECTIONS IS 1500MM !"
            End If
            If midrailHeight1 > 0 AndAlso midrailHeight2 > 0 Then
                If midrailHeight1 = midrailHeight2 Then Return "MIDRAIL HEIGHT IS IN THE SAME POSITION. PLEASE CHANGE MIDRAIL HEIGHT POSITION 2 !"
                If midrailHeight1 > midrailHeight2 Then Return "THE HEIGHT OF MIDRAIL 1 SHOULD NOT BE GREATER THAN THE HEIGHT OF MIDRAIL 2 !"

                If midrailHeight1 > 1500 Then Return "MAXIMUM ONE SECTION IS 1500MM !"
                If midrailHeight2 - midrailHeight1 > 1500 Then Return "MAXIMUM ONE SECTION IS 1500MM !"
                If drop - midrailHeight2 > 1500 Then Return "MAXIMUM ONE SECTION IS 1500MM !"
            End If
        End If

        If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" OrElse blindName = "Track Bi-fold" Then
            If String.IsNullOrEmpty(data.hingecolour) Then Return "HINGE COLOUR IS REQUIRED !"
        End If

        If blindName = "Track Sliding" OrElse blindName = "Track Sliding Single Track" Then
            If data.joinedpanels = "Yes" AndAlso String.IsNullOrEmpty(data.hingecolour) Then
                Return "HINGE COLOUR IS REQUIRED !"
            End If
            If Not String.IsNullOrEmpty(data.customheaderlength) Then
                If Not Integer.TryParse(data.customheaderlength, headerLength) OrElse headerLength < 0 Then Return "PLEASE CHECK YOUR CUSTOM HEADER LENGTH !"
                If headerLength > 0 AndAlso headerLength > width * 2 Then
                    Return "MINIMUM CUSTOM HEADER LENGTH IS 2x FROM YOUR WIDTH !"
                End If
            End If
        End If

        Dim layoutCode As String = data.layoutcode
        If data.layoutcode = "Other" Then layoutCode = data.layoutcodecustom

        If Not blindName = "Panel Only" Then
            If String.IsNullOrEmpty(data.layoutcode) Then Return "LAYOUT CODE IS REQUIRED !"
            If data.layoutcode = "Other" AndAlso String.IsNullOrEmpty(data.layoutcodecustom) Then Return "CUSTOM LAYOUT CODE IS REQUIRED !"

            If blindName = "Hinged" Then
                If layoutCode.Contains("LL") OrElse layoutCode.Contains("RR") Then Return "YOUR LAYOUT CODE CANNOT BE USED !"
            End If

            If blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("LLL") OrElse layoutCode.Contains("RRR") Then Return "YOUR LAYOUT CODE CANNOT BE USED !"
            End If

            If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("RL") Then Return "RL LAYOUT CODE CANNOT BE USED. YOU MUST PUT T BETWEEN RL, THAT IS TO BECOME RTL !"

                Dim checkLayoutD As Boolean = orderClass.CheckStringLayoutD(layoutCode)
                If checkLayoutD = False Then Return "A DASH (-) IS REQUIRED BEFORE OR AFTER THE LATTER D !"
            End If

            If blindName = "Track Bi-fold" Then
                Dim stringL As Integer = layoutCode.Split("L").Length - 1
                Dim stringR As Integer = layoutCode.Split("R").Length - 1
                If Not stringL Mod 2 = 0 Then Return "LAYOUT CODE L SHOULD NOT BE ODD !"
                If Not stringR Mod 2 = 0 Then Return "LAYOUT CODE R SHOULD NOT BE ODD !"
            End If

            If blindName = "Track Sliding" Then
                If InStr(layoutCode, "M") > 0 AndAlso Not data.louvreposition = "Closed" Then Return "LOUVRE POSITION SHOULD BE CLOSED !"
            End If

            If String.IsNullOrEmpty(data.frametype) Then Return "FRAME TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.frameleft) Then Return "LEFT FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.frameright) Then Return "RIGHT FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.frametop) Then Return "TOP FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.framebottom) Then Return "BOTTOM FRAME IS REQUIRED !"
        End If

        If blindName = "Track Bi-fold" OrElse blindName = "Track Sliding" OrElse blindName = "Track Sliding Single Track" Then
            If String.IsNullOrEmpty(data.bottomtracktype) Then Return "BOTTOM TRACK TYPE IS REQUIRED !"
        End If

        If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
            If (data.frametype = "Small Bullnose Z Frame" OrElse data.frametype = "Large Bullnose Z Frame" OrElse data.frametype = "Colonial Z Frame" OrElse data.frametype = "Regal Z Frame") AndAlso Not String.IsNullOrEmpty(data.buildout) AndAlso String.IsNullOrEmpty(data.buildoutposition) Then
                Return "BUILDOUT POSITION IS REQUIRED !"
            End If
        End If

        If Not String.IsNullOrEmpty(data.gap1) Then
            If Not Integer.TryParse(data.gap1, gap1) OrElse gap1 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 1 !"
        End If
        If Not String.IsNullOrEmpty(data.gap2) Then
            If Not Integer.TryParse(data.gap2, gap2) OrElse gap2 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 2 !"
        End If
        If Not String.IsNullOrEmpty(data.gap3) Then
            If Not Integer.TryParse(data.gap3, gap3) OrElse gap3 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 3 !"
        End If
        If Not String.IsNullOrEmpty(data.gap4) Then
            If Not Integer.TryParse(data.gap4, gap4) OrElse gap4 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 4 !"
        End If
        If Not String.IsNullOrEmpty(data.gap5) Then
            If Not Integer.TryParse(data.gap5, gap5) OrElse gap5 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 5 !"
        End If

        If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
            If Not String.IsNullOrEmpty(data.horizontaltpostheight) Then
                If Not Integer.TryParse(data.horizontaltpostheight, horizontalHeight) OrElse horizontalHeight < 0 Then Return "PLEASE CHECK YOUR HORIZONTAL T-POST HEIGHT !"
                If horizontalHeight > 0 AndAlso String.IsNullOrEmpty(data.horizontaltpost) Then
                    Return "HORIZONTAL T-POST (YES / NO POST) IS REQUIRED !"
                End If
            End If
        End If

        If blindName = "Panel Only" AndAlso String.IsNullOrEmpty(data.panelqty) Then Return "PANEL QTY IS REQUIRED !"

        If String.IsNullOrEmpty(data.tiltrodtype) Then Return "TILTROD TYPE IS REQUIRED !"

        If data.tiltrodsplit = "Other" Then
            If Not String.IsNullOrEmpty(data.splitheight1) Then
                If Not Integer.TryParse(data.splitheight1, splitHeight1) OrElse splitHeight1 <= 0 Then Return "PLEASE CHECK YOUR SPLIT HEIGHT 1 ORDER !"
            End If
            If Not String.IsNullOrEmpty(data.splitheight2) Then
                If Not Integer.TryParse(data.splitheight2, splitHeight2) OrElse splitHeight2 <= 0 Then Return "PLEASE CHECK YOUR SPLIT HEIGHT 21 ORDER !"
            End If
        End If

        Dim datacheckPanelQty As String() = {blindName, data.panelqty, layoutCode, horizontalHeight}
        panelQty = orderClass.GetPanelQty(datacheckPanelQty)

        'DEDUCTIONS
        If designName = "Skyline Shutter Ocean" AndAlso roleName = "Customer" Then
            Dim dataWidthDeductions As Object() = {blindName, "All", width, data.mounting, layoutCode, data.frametype, data.frameleft, data.frameright, panelQty}
            Dim dataHeightDeductions As String() = {blindName, drop, data.mounting, data.frametype, data.frametop, data.framebottom, data.bottomtracktype, data.horizontaltpost}

            Dim widthDeductions As Decimal = orderClass.WidthDeductShutter(dataWidthDeductions)
            Dim panelWidth As Decimal = widthDeductions / panelQty

            Dim heightDeduct As Decimal = orderClass.HeightDeductShutter(dataHeightDeductions)
            Dim panelHeight As Integer = heightDeduct

            If blindName = "Panel Only" Then
                If width < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If width > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"

                If drop < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If drop < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If drop < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If drop > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Hinged" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"

                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"

                If panelHeight > 1900 AndAlso blindName = "Hinged Bi-fold" AndAlso (data.framebottom = "No" OrElse data.framebottom = "Light Block" OrElse data.framebottom = "L Striker Plate") Then
                    Return "MAXIMUM PANEL HEIGHT IS 1900MM !"
                End If

                If panelHeight > 2500 Then
                    Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
                End If
            End If

            If blindName = "Hinged Bi-fold" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"
                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"

                If panelHeight > 1900 AndAlso data.framebottom = "No" Then Return "MAXIMUM PANEL HEIGHT IS 1900MM !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Track Bi-fold" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 600 Then Return "MAXIMUM PANEL WIDTH IS 600MM !"

                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Track Sliding" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MINIMUM PANEL WIDTH IS 900MM !"
                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Track Sliding Single Track" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MINIMUM PANEL WIDTH IS 900MM !"
                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Fixed" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"
                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"

                If blindName = "Fixed" AndAlso data.frametype = "U Channel" AndAlso panelHeight > 2527 Then Return "MAXIMUM PANEL HEIGHT IS 2527MM !"
                If blindName = "Fixed" AndAlso data.frametype = "19x19 Light Block" AndAlso panelHeight > 2506 Then Return "MAXIMUM PANEL HEIGHT IS 2506MM !"
            End If
        End If

        'GAP POSITION
        If designName = "Skyline Shutter Ocean" AndAlso roleName = "Customer" Then
            If String.IsNullOrEmpty(data.samesizepanel) Then
                If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
                    Dim pemisah As Char() = {"T"c, "C"c, "B"c, "G"c}

                    Dim gaps As Integer() = {gap1, gap2, gap3, gap4, gap5}
                    Dim totalWidth As Integer = width

                    Dim sections As New List(Of String)
                    Dim startIndex As Integer = 0
                    Dim totalPemisah As Integer = 0

                    For i As Integer = 1 To layoutCode.Length - 1
                        If pemisah.Contains(layoutCode(i)) Then
                            totalPemisah += 1
                            sections.Add(layoutCode.Substring(startIndex, i - startIndex + 1))
                            startIndex = i
                        End If
                    Next

                    If startIndex < layoutCode.Length Then
                        sections.Add(layoutCode.Substring(startIndex))
                    End If

                    Dim sumGapUsed As Integer = 0

                    For idx As Integer = 0 To sections.Count - 1
                        Dim section As String = sections(idx)
                        Dim panelCount As Integer = section.Count(Function(ch) "LRFM".Contains(ch))

                        Dim currentGap As Integer

                        If idx = sections.Count - 1 Then
                            currentGap = totalWidth - sumGapUsed
                        Else
                            currentGap = If(idx < gaps.Length, gaps(idx), 0)
                            sumGapUsed += currentGap
                        End If

                        If currentGap <= 0 Then
                            Return String.Format("GAP 1 IS REQUIRED !", idx + 1)
                            Exit For
                        End If

                        Dim dataGap As Object() = {blindName, "Gap", currentGap, data.mounting, section, data.frametype, data.frameleft, data.frameright, panelCount}

                        Dim widthDeduct As Decimal = orderClass.WidthDeductShutter(dataGap)

                        If widthDeduct / panelCount < 200 Then
                            Return String.Format("MINIMUM PANEL WIDTH IS 200MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                            Exit For
                        End If
                        If blindName = "Hinged Bi-fold" AndAlso widthDeduct / panelCount > 650 Then
                            Return String.Format("MAXIMUM PANEL WIDTH FOR HINGED BI-FOLD IS 650MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                            Exit For
                        End If
                        If widthDeduct / panelCount > 900 Then
                            Return String.Format("MAXIMUM PANEL WIDTH IS 900MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                            Exit For
                        End If
                    Next
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If blindName = "Panel Only" Then
            data.louvreposition = String.Empty
            data.joinedpanels = String.Empty
            data.hingecolour = String.Empty
            data.semiinside = String.Empty

            data.layoutcode = String.Empty
            data.layoutcodecustom = String.Empty
            data.frametype = String.Empty
            data.frameleft = String.Empty : data.frameright = String.Empty
            data.frametop = String.Empty : data.framebottom = String.Empty
            data.bottomtracktype = String.Empty
            data.bottomtrackrecess = String.Empty
            data.buildout = String.Empty
            data.buildoutposition = String.Empty
            data.samesizepanel = String.Empty
            data.horizontaltpost = String.Empty

            data.reversehinged = String.Empty
            data.pelmetflat = String.Empty
            data.extrafascia = String.Empty
            data.hingesloose = String.Empty
            data.cutout = String.Empty
            data.specialshape = String.Empty
            data.templateprovided = String.Empty

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            horizontalHeight = 0
            gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            headerLength = 0
            trackQty = 0
            trackLength = 0
            hingeQtyPerPanel = 0
            panelQtyWithHinge = 0
        End If

        If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
            data.louvreposition = String.Empty
            data.joinedpanels = String.Empty
            data.semiinside = String.Empty

            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = String.Empty
            End If

            data.bottomtracktype = String.Empty
            data.bottomtrackrecess = String.Empty

            If data.frametype = "No Frame" Then data.buildout = String.Empty
            If data.buildout = "" Then data.buildoutposition = String.Empty

            If Not (layoutCode.Contains("T") OrElse layoutCode.Contains("B") OrElse layoutCode.Contains("C") OrElse layoutCode.Contains("G")) Then
                data.samesizepanel = String.Empty
                gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            End If

            If data.samesizepanel = "Yes" Then
                gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            End If

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            If horizontalHeight = 0 Then data.horizontaltpost = ""
            data.reversehinged = String.Empty
            data.pelmetflat = String.Empty
            data.extrafascia = String.Empty

            If designName = "Skyline Shutter Express" Then
                data.cutout = String.Empty
                data.specialshape = String.Empty
            End If
            If data.specialshape = "" Then data.templateprovided = String.Empty

            headerLength = 0

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5

            Dim countL As Integer = 0
            Dim countR As Integer = 0
            countL = layoutCode.Split("L").Length - 1
            countR = layoutCode.Split("R").Length - 1

            panelQtyWithHinge = countL + countR
        End If

        If blindName = "Track Bi-fold" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.horizontaltpost = ""
            data.horizontaltpostheight = "0"
            data.buildout = ""
            data.buildoutposition = ""
            If data.mounting = "Outside" Then data.semiinside = ""

            data.customheaderlength = 0
            trackLength = width
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = String.Empty
            End If
            If data.bottomtracktype = "U Track" Then data.bottomtrackrecess = "Yes"

            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = String.Empty

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If
            If data.specialshape = "" Then data.templateprovided = ""

            Dim result1 As Integer = 0
            Dim parts As String() = layoutCode.Split("/"c)
            If parts.Length > 0 Then
                result1 = orderClass.CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = orderClass.CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            panelQtyWithHinge = result1 + result2

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        If blindName = "Track Sliding" Then
            If data.mounting = "Outside" Then data.semiinside = ""
            If data.joinedpanels = "" Then
                data.hingecolour = ""
                data.hingesloose = ""
            End If

            If Not data.layoutcode = "Other" Then data.layoutcodecustom = ""

            If data.bottomtracktype = "U Track" Then data.bottomtrackrecess = "Yes"

            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = ""

            data.horizontaltpost = "" : data.horizontaltpostheight = 0
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight1 = 0
            End If
            data.reversehinged = ""
            If data.specialshape = "" Then data.templateprovided = ""

            Dim countM As Integer = 0
            countM = layoutCode.Split("M").Length - 1

            trackQty = 2
            If countM > 0 Then trackQty = 3

            Dim countFF As Integer = 0
            Dim countMM As Integer = 0
            Dim countBB As Integer = 0
            countFF = layoutCode.Split("FF").Length - 1
            countMM = layoutCode.Split("MM").Length - 1
            countBB = layoutCode.Split("BB").Length - 1

            panelQtyWithHinge = countFF + countMM + countBB

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        If blindName = "Track Sliding Single Track" Then
            If data.mounting = "Outside" Then
                data.semiinside = ""
            End If
            data.louvreposition = ""
            If data.joinedpanels = "" Then
                data.hingecolour = ""
                data.hingesloose = ""
            End If
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If

            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = ""

            If data.bottomtracktype = "U Track" Then
                data.bottomtrackrecess = "Yes"
            End If

            data.horizontaltpostheight = 0 : data.horizontaltpost = ""
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If
            data.reversehinged = ""
            If data.specialshape = "" Then
                data.templateprovided = ""
            End If
            trackQty = 1

            Dim countFF As Integer = 0
            Dim countMM As Integer = 0
            Dim countBB As Integer = 0
            countFF = layoutCode.Split("FF").Length - 1
            countMM = layoutCode.Split("MM").Length - 1
            countBB = layoutCode.Split("BB").Length - 1

            panelQtyWithHinge = countFF + countMM + countBB

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        If blindName = "Fixed" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.hingecolour = ""

            data.semiinside = ""
            data.customheaderlength = ""
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If
            data.bottomtracktype = "" : data.bottomtrackrecess = ""
            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = ""

            data.horizontaltpostheight = 0 : data.horizontaltpost = ""

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            data.reversehinged = ""
            data.pelmetflat = ""
            data.extrafascia = ""
            data.hingesloose = ""
            If data.specialshape = "" Then data.templateprovided = ""
        End If

        Dim groupName As String = String.Empty
        If designName = "Skyline Shutter Express" Then
            groupName = designName
        End If

        If designName = "Skyline Shutter Ocean" Then
            groupName = designName
            If data.cutout = "Yes" Then
                groupName = String.Format("{0} - French Door Cut-Out", designName)
            End If
        End If

        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)

        Dim squareMetre As Decimal = width * drop / 1000000
        Dim linearMetre As Decimal = width / 1000

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, LayoutCode, LayoutCodeCustom, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, BottomTrackRecess, Buildout, BuildoutPosition, PanelQty, TrackQty, TrackLength, SameSizePanel, HingeQtyPerPanel, PanelQtyWithHinge, Gap1, Gap2, Gap3, Gap4, Gap5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, DoorCutOut, SpecialShape, TemplateProvided, SquareMetre, LinearMetre, TotalItems, Notes, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @SemiInsideMount, @LouvreSize, @LouvrePosition, @HingeColour, @MidrailHeight1, @MidrailHeight2, @MidrailCritical, @LayoutCode, @LayoutCodeCustom, @CustomHeaderLength, @FrameType, @FrameLeft, @FrameRight, @FrameTop, @FrameBottom, @BottomTrackType, @BottomTrackRecess, @Buildout, @BuildoutPosition, @PanelQty, @TrackQty, @TrackLength, @SameSizePanel, @HingeQtyPerPanel, @PanelQtyWithHinge, @Gap1, @Gap2, @Gap3, @Gap4, @Gap5, @HorizontalTPost, @HorizontalTPostHeight, @JoinedPanels, @ReverseHinged, @PelmetFlat, @ExtraFascia, @HingesLoose, @TiltrodType, @TiltrodSplit, @SplitHeight1, @SplitHeight2, @DoorCutOut, @SpecialShape, @TemplateProvided, @SquareMetre, @LinearMetre, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinside)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                        myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                        myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                        myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailcritical)
                        myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@LayoutCodeCustom", data.layoutcodecustom)
                        myCmd.Parameters.AddWithValue("@CustomHeaderLength", headerLength)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.frameleft)
                        myCmd.Parameters.AddWithValue("@FrameRight", data.frameright)
                        myCmd.Parameters.AddWithValue("@FrameTop", data.frametop)
                        myCmd.Parameters.AddWithValue("@FrameBottom", data.framebottom)
                        myCmd.Parameters.AddWithValue("@BottomTrackType", data.bottomtracktype)
                        myCmd.Parameters.AddWithValue("@BottomTrackRecess", data.bottomtrackrecess)
                        myCmd.Parameters.AddWithValue("@Buildout", data.buildout)
                        myCmd.Parameters.AddWithValue("@BuildoutPosition", data.buildoutposition)
                        myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                        myCmd.Parameters.AddWithValue("@TrackQty", trackQty)
                        myCmd.Parameters.AddWithValue("@TrackLength", trackLength)
                        myCmd.Parameters.AddWithValue("@SameSizePanel", data.samesizepanel)
                        myCmd.Parameters.AddWithValue("@HingeQtyPerPanel", hingeQtyPerPanel)
                        myCmd.Parameters.AddWithValue("@PanelQtyWithHinge", panelQtyWithHinge)
                        myCmd.Parameters.AddWithValue("@Gap1", gap1)
                        myCmd.Parameters.AddWithValue("@Gap2", gap2)
                        myCmd.Parameters.AddWithValue("@Gap3", gap3)
                        myCmd.Parameters.AddWithValue("@Gap4", gap4)
                        myCmd.Parameters.AddWithValue("@Gap5", gap5)
                        myCmd.Parameters.AddWithValue("@HorizontalTPost", data.horizontaltpost)
                        myCmd.Parameters.AddWithValue("@HorizontalTPostHeight", horizontalHeight)
                        myCmd.Parameters.AddWithValue("@JoinedPanels", data.joinedpanels)
                        myCmd.Parameters.AddWithValue("@ReverseHinged", data.reversehinged)
                        myCmd.Parameters.AddWithValue("@PelmetFlat", data.pelmetflat)
                        myCmd.Parameters.AddWithValue("@ExtraFascia", data.extrafascia)
                        myCmd.Parameters.AddWithValue("@HingesLoose", data.hingesloose)
                        myCmd.Parameters.AddWithValue("@TiltrodType", data.tiltrodtype)
                        myCmd.Parameters.AddWithValue("@TiltrodSplit", data.tiltrodsplit)
                        myCmd.Parameters.AddWithValue("@SplitHeight1", splitHeight1)
                        myCmd.Parameters.AddWithValue("@SplitHeight2", splitHeight2)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.cutout)
                        myCmd.Parameters.AddWithValue("@SpecialShape", data.specialshape)
                        myCmd.Parameters.AddWithValue("@TemplateProvided", data.templateprovided)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@TotalItems", panelQty)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next

            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, SemiInsideMount=@SemiInsideMount, LouvreSize=@LouvreSize, LouvrePosition=@LouvrePosition, HingeColour=@HingeColour, MidrailHeight1=@MidrailHeight1, MidrailHeight2=@MidrailHeight2, MidrailCritical=@MidrailCritical, LayoutCode=@LayoutCode, LayoutCodeCustom=@LayoutCodeCustom, CustomHeaderLength=@CustomHeaderLength, FrameType=@FrameType, FrameLeft=@FrameLeft, FrameRight=@FrameRight, FrameTop=@FrameTop, FrameBottom=@FrameBottom, BottomTrackType=@BottomTrackType, BottomTrackRecess=@BottomTrackRecess, Buildout=@Buildout, BuildoutPosition=@BuildoutPosition, PanelQty=@PanelQty, TrackQty=@TrackQty, TrackLength=@TrackLength, SameSizePanel=@SameSizePanel, HingeQtyPerPanel=@HingeQtyPerPanel, PanelQtyWithHinge=@PanelQtyWithHinge, Gap1=@Gap1, Gap2=@Gap2, Gap3=@Gap3, Gap4=@Gap4, Gap5=@Gap5, HorizontalTPost=@HorizontalTPost, HorizontalTPostHeight=@HorizontalTPostHeight, JoinedPanels=@JoinedPanels, ReverseHinged=@ReverseHinged, PelmetFlat=@PelmetFlat, ExtraFascia=@ExtraFascia, HingesLoose=@HingesLoose, TiltrodType=@TiltrodType, TiltrodSplit=@TiltrodSplit, SplitHeight1=@SplitHeight1, SplitHeight2=@SplitHeight2, DoorCutOut=@DoorCutOut, SpecialShape=@SpecialShape, TemplateProvided=@TemplateProvided, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, MarkUp=@MarkUp, TotalItems=@TotalItems, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", 1)
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinside)
                    myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                    myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                    myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                    myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                    myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                    myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailcritical)
                    myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                    myCmd.Parameters.AddWithValue("@LayoutCodeCustom", data.layoutcodecustom)
                    myCmd.Parameters.AddWithValue("@CustomHeaderLength", headerLength)
                    myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                    myCmd.Parameters.AddWithValue("@FrameLeft", data.frameleft)
                    myCmd.Parameters.AddWithValue("@FrameRight", data.frameright)
                    myCmd.Parameters.AddWithValue("@FrameTop", data.frametop)
                    myCmd.Parameters.AddWithValue("@FrameBottom", data.framebottom)
                    myCmd.Parameters.AddWithValue("@BottomTrackType", data.bottomtracktype)
                    myCmd.Parameters.AddWithValue("@BottomTrackRecess", data.bottomtrackrecess)
                    myCmd.Parameters.AddWithValue("@Buildout", data.buildout)
                    myCmd.Parameters.AddWithValue("@BuildoutPosition", data.buildoutposition)
                    myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                    myCmd.Parameters.AddWithValue("@TrackQty", trackQty)
                    myCmd.Parameters.AddWithValue("@TrackLength", trackLength)
                    myCmd.Parameters.AddWithValue("@SameSizePanel", data.samesizepanel)
                    myCmd.Parameters.AddWithValue("@HingeQtyPerPanel", hingeQtyPerPanel)
                    myCmd.Parameters.AddWithValue("@PanelQtyWithHinge", panelQtyWithHinge)
                    myCmd.Parameters.AddWithValue("@Gap1", gap1)
                    myCmd.Parameters.AddWithValue("@Gap2", gap2)
                    myCmd.Parameters.AddWithValue("@Gap3", gap3)
                    myCmd.Parameters.AddWithValue("@Gap4", gap4)
                    myCmd.Parameters.AddWithValue("@Gap5", gap5)
                    myCmd.Parameters.AddWithValue("@HorizontalTPost", data.horizontaltpost)
                    myCmd.Parameters.AddWithValue("@HorizontalTPostHeight", horizontalHeight)
                    myCmd.Parameters.AddWithValue("@JoinedPanels", data.joinedpanels)
                    myCmd.Parameters.AddWithValue("@ReverseHinged", data.reversehinged)
                    myCmd.Parameters.AddWithValue("@PelmetFlat", data.pelmetflat)
                    myCmd.Parameters.AddWithValue("@ExtraFascia", data.extrafascia)
                    myCmd.Parameters.AddWithValue("@HingesLoose", data.hingesloose)
                    myCmd.Parameters.AddWithValue("@TiltrodType", data.tiltrodtype)
                    myCmd.Parameters.AddWithValue("@TiltrodSplit", data.tiltrodsplit)
                    myCmd.Parameters.AddWithValue("@SplitHeight1", splitHeight1)
                    myCmd.Parameters.AddWithValue("@SplitHeight2", splitHeight2)
                    myCmd.Parameters.AddWithValue("@DoorCutOut", data.cutout)
                    myCmd.Parameters.AddWithValue("@SpecialShape", data.specialshape)
                    myCmd.Parameters.AddWithValue("@TemplateProvided", data.templateprovided)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@TotalItems", panelQty)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function EvolveProccess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim designName As String = orderClass.GetDesignName(data.designid)
        Dim blindName As String = orderClass.GetBlindName(data.blindtype)

        Dim roleName As String = orderClass.GetUserRoleName(data.loginid)

        Dim width As Integer = 0
        Dim drop As Integer = 0
        Dim midrailHeight1 As Integer = 0
        Dim midrailHeight2 As Integer = 0
        Dim headerLength As Integer = 0
        Dim horizontalHeight As Integer = 0

        Dim splitHeight1 As Integer = 0
        Dim splitHeight2 As Integer = 0

        Dim gap1 As Integer = 0
        Dim gap2 As Integer = 0
        Dim gap3 As Integer = 0
        Dim gap4 As Integer = 0
        Dim gap5 As Integer = 0

        Dim panelQty As Integer = 0
        Dim trackQty As Integer = 0
        Dim trackLength As Integer = 0
        Dim hingeQtyPerPanel As Integer = 0
        Dim panelQtyWithHinge As Integer = 0

        Dim markup As Integer

        If String.IsNullOrEmpty(data.blindtype) Then Return "SHUTTER TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "COLOUR IS REQUIRED !"

        Dim qty As Integer
        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        If String.IsNullOrEmpty(data.drop) Then Return "HEIGHT IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR HEIGHT ORDER !"

        If String.IsNullOrEmpty(data.louvresize) Then Return "LOUVRE SIZE IS REQUIRED !"

        If blindName = "Track Sliding" Then
            If String.IsNullOrEmpty(data.louvreposition) Then Return "SLIDING LOUVRE POSITION IS REQUIRED !"
        End If

        If Not String.IsNullOrEmpty(data.midrailheight1) Then
            If Not Integer.TryParse(data.midrailheight1, midrailHeight1) OrElse midrailHeight1 < 0 Then Return "PLEASE CHECK YOUR MIDRAIL HEIGHT 1 ORDER !"
        End If
        If Not String.IsNullOrEmpty(data.midrailheight2) Then
            If Not Integer.TryParse(data.midrailheight2, midrailHeight2) OrElse midrailHeight2 < 0 Then Return "PLEASE CHECK YOUR MIDRAIL HEIGHT 2 ORDER !"
        End If

        If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" OrElse blindName = "Track Bi-fold" Then
            If String.IsNullOrEmpty(data.hingecolour) Then Return "HINGE COLOUR IS REQUIRED !"
        End If

        If blindName = "Track Sliding" OrElse blindName = "Track Sliding Single Track" Then
            If data.joinedpanels = "Yes" AndAlso String.IsNullOrEmpty(data.hingecolour) Then
                Return "HINGE COLOUR IS REQUIRED !"
            End If
            If Not String.IsNullOrEmpty(data.customheaderlength) Then
                If Not Integer.TryParse(data.customheaderlength, headerLength) OrElse headerLength < 0 Then Return "PLEASE CHECK YOUR CUSTOM HEADER LENGTH !"
                If headerLength > 0 AndAlso headerLength > width * 2 Then
                    Return "MINIMUM CUSTOM HEADER LENGTH IS 2x FROM YOUR WIDTH !"
                End If
            End If
        End If

        Dim layoutCode As String = data.layoutcode
        If data.layoutcode = "Other" Then layoutCode = data.layoutcodecustom

        If Not blindName = "Panel Only" Then
            If String.IsNullOrEmpty(data.layoutcode) Then Return "LAYOUT CODE IS REQUIRED !"
            If data.layoutcode = "Other" AndAlso String.IsNullOrEmpty(data.layoutcodecustom) Then Return "CUSTOM LAYOUT CODE IS REQUIRED !"

            If blindName = "Hinged" Then
                If layoutCode.Contains("LL") OrElse layoutCode.Contains("RR") Then Return "YOUR LAYOUT CODE CANNOT BE USED !"
            End If

            If blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("LLL") OrElse layoutCode.Contains("RRR") Then Return "YOUR LAYOUT CODE CANNOT BE USED !"
            End If

            If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("RL") Then Return "RL LAYOUT CODE CANNOT BE USED. YOU MUST PUT T BETWEEN RL, THAT IS TO BECOME RTL !"

                Dim checkLayoutD As Boolean = orderClass.CheckStringLayoutD(layoutCode)
                If checkLayoutD = False Then Return "A DASH (-) IS REQUIRED BEFORE OR AFTER THE LATTER D !"
            End If

            If blindName = "Track Bi-fold" Then
                Dim stringL As Integer = layoutCode.Split("L").Length - 1
                Dim stringR As Integer = layoutCode.Split("R").Length - 1
                If Not stringL Mod 2 = 0 Then Return "LAYOUT CODE L SHOULD NOT BE ODD !"
                If Not stringR Mod 2 = 0 Then Return "LAYOUT CODE R SHOULD NOT BE ODD !"
            End If

            If blindName = "Track Sliding" Then
                If InStr(layoutCode, "M") > 0 AndAlso Not data.louvreposition = "Closed" Then Return "LOUVRE POSITION SHOULD BE CLOSED !"
            End If

            If String.IsNullOrEmpty(data.frametype) Then Return "FRAME TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.frameleft) Then Return "LEFT FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.frameright) Then Return "RIGHT FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.frametop) Then Return "TOP FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.framebottom) Then Return "BOTTOM FRAME IS REQUIRED !"
        End If

        If blindName = "Track Bi-fold" OrElse blindName = "Track Sliding" OrElse blindName = "Track Sliding Single Track" Then
            If data.framebottom = "No" AndAlso String.IsNullOrEmpty(data.bottomtracktype) Then
                Return "BOTTOM TRACK TYPE IS REQUIRED !"
            End If
        End If

        If Not String.IsNullOrEmpty(data.gap1) Then
            If Not Integer.TryParse(data.gap1, gap1) OrElse gap1 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 1 !"
        End If
        If Not String.IsNullOrEmpty(data.gap2) Then
            If Not Integer.TryParse(data.gap2, gap2) OrElse gap2 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 2 !"
        End If
        If Not String.IsNullOrEmpty(data.gap3) Then
            If Not Integer.TryParse(data.gap3, gap3) OrElse gap3 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 3 !"
        End If
        If Not String.IsNullOrEmpty(data.gap4) Then
            If Not Integer.TryParse(data.gap4, gap4) OrElse gap4 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 4 !"
        End If
        If Not String.IsNullOrEmpty(data.gap5) Then
            If Not Integer.TryParse(data.gap5, gap5) OrElse gap5 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 5 !"
        End If

        If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
            If Not String.IsNullOrEmpty(data.horizontaltpostheight) Then
                If Not Integer.TryParse(data.horizontaltpostheight, horizontalHeight) OrElse horizontalHeight < 0 Then Return "PLEASE CHECK YOUR HORIZONTAL T-POST HEIGHT !"
                If horizontalHeight > 0 AndAlso String.IsNullOrEmpty(data.horizontaltpost) Then
                    Return "HORIZONTAL T-POST (YES / NO POST) IS REQUIRED !"
                End If
            End If
        End If

        If blindName = "Panel Only" AndAlso String.IsNullOrEmpty(data.panelqty) Then Return "PANEL QTY IS REQUIRED !"

        If String.IsNullOrEmpty(data.tiltrodtype) Then Return "TILTROD TYPE IS REQUIRED !"

        If data.tiltrodsplit = "Other" Then
            If Not String.IsNullOrEmpty(data.splitheight1) Then
                If Not Integer.TryParse(data.splitheight1, splitHeight1) OrElse splitHeight1 <= 0 Then Return "PLEASE CHECK YOUR SPLIT HEIGHT 1 ORDER !"
            End If
            If Not String.IsNullOrEmpty(data.splitheight2) Then
                If Not Integer.TryParse(data.splitheight2, splitHeight2) OrElse splitHeight2 <= 0 Then Return "PLEASE CHECK YOUR SPLIT HEIGHT 21 ORDER !"
            End If
        End If

        Dim datacheckPanelQty As String() = {blindName, data.panelqty, layoutCode, horizontalHeight}
        panelQty = orderClass.GetPanelQty(datacheckPanelQty)

        'DEDUCTIONS
        If designName = "Evolve Shutter Ocean" AndAlso roleName = "Customer" Then
            Dim dataWidthDeductions As Object() = {blindName, "All", width, data.mounting, layoutCode, data.frametype, data.frameleft, data.frameright, panelQty}
            Dim dataHeightDeductions As String() = {blindName, drop, data.mounting, data.frametype, data.frametop, data.framebottom, data.bottomtracktype, data.horizontaltpost}

            Dim widthDeductions As Decimal = orderClass.WidthDeductShutter(dataWidthDeductions)
            Dim panelWidth As Decimal = widthDeductions / panelQty

            Dim heightDeduct As Decimal = orderClass.HeightDeductShutter(dataHeightDeductions)
            Dim panelHeight As Integer = heightDeduct

            If blindName = "Panel Only" Then
                If width < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If width > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"

                If drop < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If drop < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If drop < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If drop > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Hinged" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"

                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"

                If panelHeight > 1900 AndAlso blindName = "Hinged Bi-fold" AndAlso (data.framebottom = "No" OrElse data.framebottom = "Light Block" OrElse data.framebottom = "L Striker Plate") Then
                    Return "MAXIMUM PANEL HEIGHT IS 1900MM !"
                End If

                If panelHeight > 2500 Then
                    Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
                End If
            End If

            If blindName = "Hinged Bi-fold" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"
                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"

                If panelHeight > 1900 AndAlso data.framebottom = "No" Then Return "MAXIMUM PANEL HEIGHT IS 1900MM !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Track Bi-fold" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 600 Then Return "MAXIMUM PANEL WIDTH IS 600MM !"

                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Track Sliding" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MINIMUM PANEL WIDTH IS 900MM !"
                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Track Sliding Single Track" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MINIMUM PANEL WIDTH IS 900MM !"
                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Fixed" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"
                If panelHeight < 282 AndAlso data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 AndAlso data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 AndAlso data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"

                If blindName = "Fixed" AndAlso data.frametype = "U Channel" AndAlso panelHeight > 2527 Then Return "MAXIMUM PANEL HEIGHT IS 2527MM !"
                If blindName = "Fixed" AndAlso data.frametype = "19x19 Light Block" AndAlso panelHeight > 2506 Then Return "MAXIMUM PANEL HEIGHT IS 2506MM !"
            End If
        End If

        'GAP POSITION
        If String.IsNullOrEmpty(data.samesizepanel) AndAlso designName = "Evolve Shutter Ocean" AndAlso roleName = "Customer" Then
            If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
                Dim pemisah As Char() = {"T"c, "C"c, "B"c, "G"c}

                Dim gaps As Integer() = {gap1, gap2, gap3, gap4, gap5}
                Dim totalWidth As Integer = width

                Dim sections As New List(Of String)
                Dim startIndex As Integer = 0
                Dim totalPemisah As Integer = 0

                For i As Integer = 1 To layoutCode.Length - 1
                    If pemisah.Contains(layoutCode(i)) Then
                        totalPemisah += 1
                        sections.Add(layoutCode.Substring(startIndex, i - startIndex + 1))
                        startIndex = i
                    End If
                Next

                If startIndex < layoutCode.Length Then
                    sections.Add(layoutCode.Substring(startIndex))
                End If

                Dim sumGapUsed As Integer = 0

                For idx As Integer = 0 To sections.Count - 1
                    Dim section As String = sections(idx)
                    Dim panelCount As Integer = section.Count(Function(ch) "LRFM".Contains(ch))

                    Dim currentGap As Integer

                    If idx = sections.Count - 1 Then
                        currentGap = totalWidth - sumGapUsed
                    Else
                        currentGap = If(idx < gaps.Length, gaps(idx), 0)
                        sumGapUsed += currentGap
                    End If

                    If currentGap <= 0 Then
                        Return String.Format("GAP 1 IS REQUIRED !", idx + 1)
                        Exit For
                    End If

                    Dim dataGap As Object() = {blindName, "Gap", currentGap, data.mounting, section, data.frametype, data.frameleft, data.frameright, panelCount}

                    Dim widthDeduct As Decimal = orderClass.WidthDeductShutter(dataGap)

                    If widthDeduct / panelCount < 200 Then
                        Return String.Format("MINIMUM PANEL WIDTH IS 200MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                        Exit For
                    End If
                    If blindName = "Hinged Bi-fold" AndAlso widthDeduct / panelCount > 650 Then
                        Return String.Format("MAXIMUM PANEL WIDTH FOR HINGED BI-FOLD IS 650MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                        Exit For
                    End If
                    If widthDeduct / panelCount > 900 Then
                        Return String.Format("MAXIMUM PANEL WIDTH IS 900MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                        Exit For
                    End If
                Next
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If blindName = "Panel Only" Then
            data.louvreposition = String.Empty
            data.joinedpanels = String.Empty
            data.hingecolour = String.Empty
            data.semiinside = String.Empty

            data.layoutcode = String.Empty
            data.layoutcodecustom = String.Empty
            data.frametype = String.Empty
            data.frameleft = String.Empty : data.frameright = String.Empty
            data.frametop = String.Empty : data.framebottom = String.Empty
            data.bottomtracktype = String.Empty
            data.buildout = String.Empty
            data.buildoutposition = String.Empty
            data.samesizepanel = String.Empty
            data.horizontaltpost = String.Empty

            data.reversehinged = String.Empty
            data.pelmetflat = String.Empty
            data.extrafascia = String.Empty
            data.hingesloose = String.Empty

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            horizontalHeight = 0
            gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            headerLength = 0
            trackQty = 0
            trackLength = 0
            hingeQtyPerPanel = 0
            panelQtyWithHinge = 0
        End If

        If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
            data.louvreposition = String.Empty
            data.joinedpanels = String.Empty
            data.semiinside = String.Empty

            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = String.Empty
            End If

            data.bottomtracktype = String.Empty

            If Not data.frametype = "Insert L 49mm" Then data.buildout = ""

            If Not layoutCode.Contains("T") AndAlso Not layoutCode.Contains("B") AndAlso layoutCode.Contains("C") AndAlso layoutCode.Contains("G") Then
                data.samesizepanel = String.Empty
                gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            End If

            If data.samesizepanel = "Yes" Then
                gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            End If

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            If horizontalHeight = 0 Then data.horizontaltpost = String.Empty
            data.reversehinged = String.Empty
            data.pelmetflat = String.Empty
            data.extrafascia = String.Empty

            headerLength = 0

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5

            Dim countL As Integer = 0
            Dim countR As Integer = 0
            countL = layoutCode.Split("L").Length - 1
            countR = layoutCode.Split("R").Length - 1

            panelQtyWithHinge = countL + countR
        End If

        If blindName = "Track Bi-fold" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.horizontaltpost = ""
            data.horizontaltpostheight = "0"
            data.buildout = ""
            If data.mounting = "Outside" Then data.semiinside = ""

            data.customheaderlength = 0
            trackLength = width
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = String.Empty
            End If
            data.buildout = ""
            data.samesizepanel = String.Empty

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            Dim result1 As Integer = 0
            Dim parts As String() = layoutCode.Split("/"c)
            If parts.Length > 0 Then
                result1 = orderClass.CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = orderClass.CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            panelQtyWithHinge = result1 + result2

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        If blindName = "Track Sliding" Then
            If data.mounting = "Outside" Then data.semiinside = ""
            If data.joinedpanels = "" Then
                data.hingecolour = ""
                data.hingesloose = ""
            End If

            If Not data.layoutcode = "Other" Then data.layoutcodecustom = ""

            data.buildout = ""
            data.samesizepanel = ""

            data.horizontaltpost = "" : data.horizontaltpostheight = 0
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight1 = 0
            End If
            data.reversehinged = ""

            Dim countM As Integer = 0
            countM = layoutCode.Split("M").Length - 1

            trackQty = 2
            If countM > 0 Then trackQty = 3

            Dim countFF As Integer = 0
            Dim countMM As Integer = 0
            Dim countBB As Integer = 0
            countFF = layoutCode.Split("FF").Length - 1
            countMM = layoutCode.Split("MM").Length - 1
            countBB = layoutCode.Split("BB").Length - 1

            panelQtyWithHinge = countFF + countMM + countBB

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        If blindName = "Track Sliding Single Track" Then
            If data.mounting = "Outside" Then
                data.semiinside = ""
            End If
            data.louvreposition = ""
            If data.joinedpanels = "" Then
                data.hingecolour = ""
                data.hingesloose = ""
            End If
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If

            data.buildout = ""
            data.samesizepanel = ""

            data.horizontaltpostheight = 0 : data.horizontaltpost = ""
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If
            data.reversehinged = ""
            trackQty = 1

            Dim countFF As Integer = 0
            Dim countMM As Integer = 0
            Dim countBB As Integer = 0
            countFF = layoutCode.Split("FF").Length - 1
            countMM = layoutCode.Split("MM").Length - 1
            countBB = layoutCode.Split("BB").Length - 1

            panelQtyWithHinge = countFF + countMM + countBB

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        If blindName = "Fixed" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.hingecolour = ""

            data.semiinside = ""
            data.customheaderlength = ""
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If
            data.bottomtracktype = ""
            data.buildout = ""
            data.samesizepanel = ""

            data.horizontaltpostheight = 0 : data.horizontaltpost = ""

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            data.reversehinged = ""
            data.pelmetflat = ""
            data.extrafascia = ""
            data.hingesloose = ""
        End If

        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(designName, data.designid)

        Dim squareMetre As Decimal = width * drop / 1000000
        Dim linearMetre As Decimal = width / 1000

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, LayoutCode, LayoutCodeCustom, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, Buildout, PanelQty, TrackQty, TrackLength, SameSizePanel, HingeQtyPerPanel, PanelQtyWithHinge, Gap1, Gap2, Gap3, Gap4, Gap5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, SquareMetre, LinearMetre, TotalItems, Notes, MarkUp, Active) VALUES (@Id, @HeaderId, @ProductId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @SemiInsideMount, @LouvreSize, @LouvrePosition, @HingeColour, @MidrailHeight1, @MidrailHeight2, @MidrailCritical, @LayoutCode, @LayoutCodeCustom, @CustomHeaderLength, @FrameType, @FrameLeft, @FrameRight, @FrameTop, @FrameBottom, @BottomTrackType, @Buildout, @PanelQty, @TrackQty, @TrackLength, @SameSizePanel, @HingeQtyPerPanel, @PanelQtyWithHinge, @Gap1, @Gap2, @Gap3, @Gap4, @Gap5, @HorizontalTPost, @HorizontalTPostHeight, @JoinedPanels, @ReverseHinged, @PelmetFlat, @ExtraFascia, @HingesLoose, @TiltrodType, @TiltrodSplit, @SplitHeight1, @SplitHeight2, @SquareMetre, @LinearMetre, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinside)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                        myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                        myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                        myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailcritical)
                        myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@LayoutCodeCustom", data.layoutcodecustom)
                        myCmd.Parameters.AddWithValue("@CustomHeaderLength", headerLength)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.frameleft)
                        myCmd.Parameters.AddWithValue("@FrameRight", data.frameright)
                        myCmd.Parameters.AddWithValue("@FrameTop", data.frametop)
                        myCmd.Parameters.AddWithValue("@FrameBottom", data.framebottom)
                        myCmd.Parameters.AddWithValue("@BottomTrackType", data.bottomtracktype)
                        myCmd.Parameters.AddWithValue("@Buildout", data.buildout)
                        myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                        myCmd.Parameters.AddWithValue("@TrackQty", trackQty)
                        myCmd.Parameters.AddWithValue("@TrackLength", trackLength)
                        myCmd.Parameters.AddWithValue("@SameSizePanel", data.samesizepanel)
                        myCmd.Parameters.AddWithValue("@HingeQtyPerPanel", hingeQtyPerPanel)
                        myCmd.Parameters.AddWithValue("@PanelQtyWithHinge", panelQtyWithHinge)
                        myCmd.Parameters.AddWithValue("@Gap1", gap1)
                        myCmd.Parameters.AddWithValue("@Gap2", gap2)
                        myCmd.Parameters.AddWithValue("@Gap3", gap3)
                        myCmd.Parameters.AddWithValue("@Gap4", gap4)
                        myCmd.Parameters.AddWithValue("@Gap5", gap5)
                        myCmd.Parameters.AddWithValue("@HorizontalTPost", data.horizontaltpost)
                        myCmd.Parameters.AddWithValue("@HorizontalTPostHeight", horizontalHeight)
                        myCmd.Parameters.AddWithValue("@JoinedPanels", data.joinedpanels)
                        myCmd.Parameters.AddWithValue("@ReverseHinged", data.reversehinged)
                        myCmd.Parameters.AddWithValue("@PelmetFlat", data.pelmetflat)
                        myCmd.Parameters.AddWithValue("@ExtraFascia", data.extrafascia)
                        myCmd.Parameters.AddWithValue("@HingesLoose", data.hingesloose)
                        myCmd.Parameters.AddWithValue("@TiltrodType", data.tiltrodtype)
                        myCmd.Parameters.AddWithValue("@TiltrodSplit", data.tiltrodsplit)
                        myCmd.Parameters.AddWithValue("@SplitHeight1", splitHeight1)
                        myCmd.Parameters.AddWithValue("@SplitHeight2", splitHeight2)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@TotalItems", panelQty)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next

            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, SemiInsideMount=@SemiInsideMount, LouvreSize=@LouvreSize, LouvrePosition=@LouvrePosition, HingeColour=@HingeColour, MidrailHeight1=@MidrailHeight1, MidrailHeight2=@MidrailHeight2, MidrailCritical=@MidrailCritical, LayoutCode=@LayoutCode, LayoutCodeCustom=@LayoutCodeCustom, CustomHeaderLength=@CustomHeaderLength, FrameType=@FrameType, FrameLeft=@FrameLeft, FrameRight=@FrameRight, FrameTop=@FrameTop, FrameBottom=@FrameBottom, BottomTrackType=@BottomTrackType, Buildout=@Buildout, PanelQty=@PanelQty, TrackQty=@TrackQty, TrackLength=@TrackLength, SameSizePanel=@SameSizePanel, HingeQtyPerPanel=@HingeQtyPerPanel, PanelQtyWithHinge=@PanelQtyWithHinge, Gap1=@Gap1, Gap2=@Gap2, Gap3=@Gap3, Gap4=@Gap4, Gap5=@Gap5, HorizontalTPost=@HorizontalTPost, HorizontalTPostHeight=@HorizontalTPostHeight, JoinedPanels=@JoinedPanels, ReverseHinged=@ReverseHinged, PelmetFlat=@PelmetFlat, ExtraFascia=@ExtraFascia, HingesLoose=@HingesLoose, TiltrodType=@TiltrodType, TiltrodSplit=@TiltrodSplit, SplitHeight1=@SplitHeight1, SplitHeight2=@SplitHeight2, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, MarkUp=@MarkUp, TotalItems=@TotalItems, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", 1)
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinside)
                    myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                    myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                    myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                    myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                    myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                    myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailcritical)
                    myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                    myCmd.Parameters.AddWithValue("@LayoutCodeCustom", data.layoutcodecustom)
                    myCmd.Parameters.AddWithValue("@CustomHeaderLength", headerLength)
                    myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                    myCmd.Parameters.AddWithValue("@FrameLeft", data.frameleft)
                    myCmd.Parameters.AddWithValue("@FrameRight", data.frameright)
                    myCmd.Parameters.AddWithValue("@FrameTop", data.frametop)
                    myCmd.Parameters.AddWithValue("@FrameBottom", data.framebottom)
                    myCmd.Parameters.AddWithValue("@BottomTrackType", data.bottomtracktype)
                    myCmd.Parameters.AddWithValue("@Buildout", data.buildout)
                    myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                    myCmd.Parameters.AddWithValue("@TrackQty", trackQty)
                    myCmd.Parameters.AddWithValue("@TrackLength", trackLength)
                    myCmd.Parameters.AddWithValue("@SameSizePanel", data.samesizepanel)
                    myCmd.Parameters.AddWithValue("@HingeQtyPerPanel", hingeQtyPerPanel)
                    myCmd.Parameters.AddWithValue("@PanelQtyWithHinge", panelQtyWithHinge)
                    myCmd.Parameters.AddWithValue("@Gap1", gap1)
                    myCmd.Parameters.AddWithValue("@Gap2", gap2)
                    myCmd.Parameters.AddWithValue("@Gap3", gap3)
                    myCmd.Parameters.AddWithValue("@Gap4", gap4)
                    myCmd.Parameters.AddWithValue("@Gap5", gap5)
                    myCmd.Parameters.AddWithValue("@HorizontalTPost", data.horizontaltpost)
                    myCmd.Parameters.AddWithValue("@HorizontalTPostHeight", horizontalHeight)
                    myCmd.Parameters.AddWithValue("@JoinedPanels", data.joinedpanels)
                    myCmd.Parameters.AddWithValue("@ReverseHinged", data.reversehinged)
                    myCmd.Parameters.AddWithValue("@PelmetFlat", data.pelmetflat)
                    myCmd.Parameters.AddWithValue("@ExtraFascia", data.extrafascia)
                    myCmd.Parameters.AddWithValue("@HingesLoose", data.hingesloose)
                    myCmd.Parameters.AddWithValue("@TiltrodType", data.tiltrodtype)
                    myCmd.Parameters.AddWithValue("@TiltrodSplit", data.tiltrodsplit)
                    myCmd.Parameters.AddWithValue("@SplitHeight1", splitHeight1)
                    myCmd.Parameters.AddWithValue("@SplitHeight2", splitHeight2)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@TotalItems", panelQty)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function SampleProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)

        If String.IsNullOrEmpty(data.blindtype) Then Return "SAMPLE TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(designName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, PriceProductGroupId, Qty, Width, [Drop], TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @PriceProductGroupId, @Qty, 0, 0, 1, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next

            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Width=0, [Drop]=0, TotalItems=1, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function SaphoraDrapeProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer : Dim drop As Integer
        Dim controllength As Integer : Dim wandlength As Integer
        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty
        Dim tubeName As String = String.Empty
        Dim controlName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)
        If Not String.IsNullOrEmpty(data.controltype) Then controlName = orderClass.GetControlName(data.controltype)

        Dim customerPriceGroup As String = orderClass.GetPriceGroupByOrder(data.headerid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.tubetype) Then Return "SLAT TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.controltype) Then Return "CONTROL TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "TRACK COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If

        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If data.companyid = "2" OrElse (data.rolename = "Customer" OrElse data.rolename = "Installer") Then

        End If

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
        If data.companyid = "2" OrElse (data.rolename = "Customer" OrElse data.rolename = "Installer") Then

        End If

        If String.IsNullOrEmpty(data.stackposition) Then Return "STACK POSITION IS REQUIRED !"

        If controlName = "Chain" Then
            If String.IsNullOrEmpty(data.controlposition) Then Return "CONTROL POSITION IS REQUIRED !"
        End If
        If controlName = "Chain" AndAlso String.IsNullOrEmpty(data.chaincolour) Then
            Return "CHAIN COLOUR IS REQUIRED !"
        End If
        If controlName = "Wand" AndAlso String.IsNullOrEmpty(data.wandcolour) Then
            Return "WAND COLOUR IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.controllength) Then Return "CONTROL LENGTH IS REQUIRED !"

        If data.controllength = "Custom" Then
            If String.IsNullOrEmpty(data.controllengthvalue) Then Return "CONTROL LENGTH VALUE IS REQUIRED !"
            If Not Integer.TryParse(data.controllengthvalue, controllength) OrElse controllength < 0 Then Return "PLEASE CHECK YOUR CONTROL LENGTH ORDER !"

            Dim thisStandard As Integer = Math.Ceiling(drop * 2 / 3)
            If thisStandard > 1000 Then thisStandard = 1000
            If controllength < thisStandard Then
                If thisStandard = 1000 Then Return "MINIMUM CONTROL LENGTH IS 1000MM !"
                Return String.Format("CONTROL LENGTH MUST BE BETWEEN {0}MM - 1000MM !", thisStandard)
            End If
            If controllength > 1000 Then Return "MAXIMUM CONTROL LENGTH IS 1000MM !"
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If data.controllength = "Standard" Then
            controllength = Math.Ceiling(drop * 2 / 3)
            If controllength > 1000 Then controllength = 1000
        End If

        If controlName = "Chain" Then
            data.wandcolour = String.Empty
        End If

        If controlName = "Wand" Then
            data.chaincolour = String.Empty
            wandlength = controllength

            If data.stackposition = "Left" Then data.controlposition = "Right"
            If data.stackposition = "Right" Then data.controlposition = "Left"
            If data.stackposition = "Centre" Then data.controlposition = "Right and Left"
            If data.stackposition = "Split" Then data.controlposition = "Middle"
        End If

        Dim linearMetre As Decimal = width / 1000
        Dim squareMetre As Decimal = width * drop / 1000000

        Dim groupName As String = blindName
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, ChainId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], StackPosition, ControlPosition, ControlLength, ControlLengthValue, WandColour, WandLengthValue, BracketExtension, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @ChainId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @StackPosition, @ControlPosition, @ControlLength, @ControlLengthValue, @WandColour, @WandLengthValue, @BracketExtension, @LinearMetre, @SquareMetre, 1, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                        myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                        myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                        myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketextension)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next
            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, ChainId=@ChainId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, StackPosition=@StackPosition, ControlPosition=@ControlPosition, ControlLength=@ControlLength, ControlLengthValue=@ControlLengthValue, WandColour=@WandColour, WandLengthValue=@WandLengthValue, BracketExtension=@BracketExtension, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, TotalItems=1, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@ChainId", If(String.IsNullOrEmpty(data.chaincolour), CType(DBNull.Value, Object), data.chaincolour))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@StackPosition", data.stackposition)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllength)
                    myCmd.Parameters.AddWithValue("@WandColour", data.wandcolour)
                    myCmd.Parameters.AddWithValue("@WandLengthValue", wandlength)
                    myCmd.Parameters.AddWithValue("@BracketExtension", data.bracketextension)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function OutdoorProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer : Dim drop As Integer
        Dim controllengthvalue As Integer
        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim controlName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.controltype) Then controlName = orderClass.GetControlName(data.controltype)

        If String.IsNullOrEmpty(data.blindtype) Then Return "OUTDOOR TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.controltype) Then Return "CONTROL TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If Not String.IsNullOrEmpty(data.room) Then
            If data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
                Return "ROOM / LOCATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.room.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"
        If data.rolename = "Customer" Then

        End If

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"
        If data.rolename = "Customer" Then

        End If

        If String.IsNullOrEmpty(data.fabrictype) Then Return "FABRIC TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.fabriccolour) Then Return "FABRIC COLOUR IS REQUIRED !"
        If String.IsNullOrEmpty(data.controlposition) Then Return "CONTROL POSITION IS REQUIRED !"

        If controlName = "Crank" Then
            If String.IsNullOrEmpty(data.controllength) Then Return "CONTROL LENGTH IS REQUIRED !"
            If data.controllength = "Custom" Then
                If Not Integer.TryParse(data.controllengthvalue, controllengthvalue) OrElse controllengthvalue <= 0 Then Return "PLEASE CHECK YOUR CONTROL LENGTH ORDER !"

                If controllengthvalue > 2000 Then Return "MAXIMUM CONTROL LENGTH IS 2000MM !"
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If controlName = "Crank" Then
            If data.controllength = "Standard" Then
                controllengthvalue = Math.Ceiling(drop * 2 / 3)
            End If
        End If

        If controlName = "Motor" Then
            data.controllength = String.Empty : controllengthvalue = 0
        End If

        Dim linearMetre As Decimal = width / 1000
        Dim squareMetre As Decimal = width * drop / 1000000

        Dim priceProductGroup As String = String.Empty ' orderClass.GetPriceProductGroupId(tubeName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, FabricId, FabricColourId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], ControlPosition, ControlLength, ControlLengthValue, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @FabricId, @FabricColourId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @ControlPosition, @ControlLength, @ControlLengthValue, @LinearMetre, @SquareMetre, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                        myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                        myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                        myCmd.Parameters.AddWithValue("@ControlLengthValue", controllengthvalue)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@TotalItems", "1")
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next

            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, FabricId=@FabricId, FabricColourId=@FabricColourId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, ControlPosition=@ControlPosition, ControlLength=@ControlLength, ControlLengthValue=@ControlLengthValue, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, TotalItems=@TotalItems, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(data.fabrictype), CType(DBNull.Value, Object), data.fabrictype))
                    myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(data.fabriccolour), CType(DBNull.Value, Object), data.fabriccolour))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@ControlPosition", data.controlposition)
                    myCmd.Parameters.AddWithValue("@ControlLength", data.controllength)
                    myCmd.Parameters.AddWithValue("@ControlLengthValue", controllengthvalue)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@TotalItems", "1")
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function DoorProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer : Dim widthb As Integer : Dim widthc As Integer
        Dim drop As Integer
        Dim handlelength As Integer
        Dim anglelength As Integer
        Dim toptracklength As Integer
        Dim bottomtracklength As Integer
        Dim receiverlength As Integer

        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty
        Dim tubeName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)
        If Not String.IsNullOrEmpty(data.tubetype) Then tubeName = orderClass.GetTubeName(data.tubetype)

        Dim roleName As String = orderClass.GetUserRoleName(data.loginid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "DOOR TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.tubetype) Then Return "MECHANISM TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PRODUCT IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If

        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "TOP WIDTH IS REQUIRED !"
        If String.IsNullOrEmpty(data.widthb) Then Return "MIDDLE WIDTH IS REQUIRED !"
        If String.IsNullOrEmpty(data.widthc) Then Return "BOTTOM WIDTH IS REQUIRED !"

        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR TOP WIDTH ORDER !"
        If Not Integer.TryParse(data.widthb, widthb) OrElse widthb <= 0 Then Return "PLEASE CHECK YOUR MIDDLE WIDTH ORDER !"
        If Not Integer.TryParse(data.widthc, widthc) OrElse widthc <= 0 Then Return "PLEASE CHECK YOUR BOTTOM WIDTH ORDER !"

        If String.IsNullOrEmpty(data.drop) Then Return "HEIGHT IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR HEIGHT ORDER !"

        If blindName = "Flyscreen" AndAlso drop > 2100 Then Return "MAXIMUM DROP IS 2100MM !"

        If blindName = "Flyscreen" AndAlso String.IsNullOrEmpty(data.meshtype) Then Return "MESH TYPE IS REQUIRED !"

        If String.IsNullOrEmpty(data.framecolour) Then Return "FRAME COLOUR IS REQUIRED !"

        If tubeName.Contains("Hinged") OrElse tubeName.Contains("Sliding") Then
            If String.IsNullOrEmpty(data.layoutcode) Then Return "LAYOUT CODE IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.midrailposition) Then Return "MIDRAIL POSITION IS REQUIRED !"

        If blindName = "Flyscreen" AndAlso (tubeName = "Sliding Single" OrElse tubeName = "Sliding Double" OrElse tubeName = "Sliding Stacker") Then
            If String.IsNullOrEmpty(data.handletype) Then Return "HANDLE TYPE IS REQUIRED !"
        End If

        If tubeName.Contains("Hinged") OrElse tubeName.Contains("Sliding") Then
            If String.IsNullOrEmpty(data.handlelength) Then Return "HANDLE LENGTH IS REQUIRED !"
            If Not Integer.TryParse(data.handlelength, handlelength) OrElse handlelength <= 0 Then Return "PLEASE CHECK YOUR HANDLE LENGTH ORDER !"
        End If

        If blindName = "Flyscreen" AndAlso String.IsNullOrEmpty(data.triplelock) Then Return "TRIPLE LOCK IS REQUIRED !"

        If tubeName.Contains("Hinged") OrElse tubeName.Contains("Sliding") Then
            If String.IsNullOrEmpty(data.bugseal) Then Return "BUG SEAL IS REQUIRED !"
            If Not String.IsNullOrEmpty(data.pettype) Then
                If String.IsNullOrEmpty(data.petposition) Then Return "PET DOOR POSITION IS REQUIRED !"
            End If
            If String.IsNullOrEmpty(data.pettype) AndAlso Not String.IsNullOrEmpty(data.petposition) Then
                Return "PET DOOR TYPE IS REQUIRED !"
            End If
            If String.IsNullOrEmpty(data.doorcloser) Then Return "DOOR CLOSER IS REQUIRED !"
        End If

        If Not String.IsNullOrEmpty(data.angletype) AndAlso String.IsNullOrEmpty(data.anglelength) Then
            Return "ANGLE LENGTH IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.angletype) AndAlso Not String.IsNullOrEmpty(data.anglelength) Then
            Return "ANGLE TYPE IS REQUIRED !"
        End If

        If Not String.IsNullOrEmpty(data.anglelength) Then
            If Not Integer.TryParse(data.anglelength, anglelength) OrElse anglelength <= 0 Then Return "PLEASE CHECK YOUR ANGLE LENGTH ORDER !"
        End If

        If tubeName.Contains("Hinged") OrElse tubeName.Contains("Sliding") Then
            If String.IsNullOrEmpty(data.beading) Then Return "BEADING IS REQUIRED !"

            If Not String.IsNullOrEmpty(data.jambtype) AndAlso String.IsNullOrEmpty(data.jambposition) Then
                Return "JAMB ADAPTOR POSITION IS REQUIRED !"
            End If

            If String.IsNullOrEmpty(data.jambtype) AndAlso Not String.IsNullOrEmpty(data.jambposition) Then
                Return "JAMB ADAPTOR TYPE IS REQUIRED !"
            End If
        End If

        If tubeName = "Hinged Double" AndAlso String.IsNullOrEmpty(data.flushbold) Then Return "FLUSH BOLD LOCATION IS REQUIRED !"

        If tubeName = "Sliding Single" OrElse tubeName = "Sliding Double" OrElse tubeName = "Sliding Stacker" Then
            If String.IsNullOrEmpty(data.interlocktype) Then Return "INTERLOCK TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.toptrack) Then Return "TOP TRACK TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.toptracklength) Then Return "TOP TRACK LENGTH IS REQUIRED !"
            If Not Integer.TryParse(data.toptracklength, toptracklength) OrElse toptracklength <= 0 Then Return "PLEASE CHECK YOUR TOP TRACK LENGTH ORDER !"

            If String.IsNullOrEmpty(data.bottomtrack) Then Return "BOTTOM TRACK TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.toptracklength) Then Return "BOTTOM TRACK LENGTH IS REQUIRED !"
            If Not Integer.TryParse(data.bottomtracklength, bottomtracklength) OrElse bottomtracklength <= 0 Then Return "PLEASE CHECK YOUR BOTTOM TRACK LENGTH ORDER !"

            If String.IsNullOrEmpty(data.receivertype) Then Return "RECEIVER CHANNEL TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.receiverlength) Then Return "RECEIVER CHANNEL LENGTH IS REQUIRED !"
            If Not Integer.TryParse(data.receiverlength, receiverlength) OrElse receiverlength <= 0 Then Return "PLEASE CHECK YOUR RECEIVER CHANNEL LENGTH ORDER !"

            If String.IsNullOrEmpty(data.slidingqty) Then Return "SLIDING ROLLER QTY IS REQUIRED !"
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If Not tubeName = "Hinged Double" Then data.flushbold = String.Empty

        If blindName = "Standard" OrElse blindName = "Safety" OrElse blindName = "Security" Then
            data.handletype = String.Empty
        End If

        If blindName = "Flyscreen" AndAlso (tubeName = "Hinged Single" OrElse tubeName = "" OrElse tubeName = "Screen Only") Then
            data.handletype = String.Empty
        End If

        Dim linearMetre As Decimal = width / 1000
        Dim squareMetre As Decimal = width * drop / 1000000
        Dim squareMetreB As Decimal = widthb * drop / 1000000
        Dim squareMetreC As Decimal = widthc * drop / 1000000

        Dim factory As String = String.Empty
        If data.framecolour.Contains("Express") Then factory = "Express"
        If data.framecolour.Contains("Regular") Then factory = "Regular"

        Dim mechanism As String = String.Empty
        If tubeName.Contains("Hinged") Then mechanism = "Hinged"
        If tubeName.Contains("Sliding") Then mechanism = "Sliding"

        Dim groupName As String = String.Format("{0} {1} {2} {3}", designName, blindName, mechanism, factory)
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)
        Dim priceProductGroupB As String = String.Empty
        If tubeName = "Hinged Double" OrElse tubeName = "Sliding Double" Then
            priceProductGroupB = orderClass.GetPriceProductGroupId(groupName, data.designid)
        End If

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, PriceProductGroupIdB, Qty, Room, Mounting, Width, WidthB, WidthC, [Drop], MeshType, FrameColour, LayoutCode, MidrailPosition, HandleType, HandleLength, TripleLock, BugSeal, PetType, PetPosition, DoorCloser, AngleType, AngleLength, Beading, JambType, JambPosition, FlushBold, InterlockType, TopTrack, TopTrackLength, BottomTrack, BottomTrackLength, Receiver, ReceiverLength, SlidingQty, LinearMetre, SquareMetre, SquareMetreB, SquareMetreC, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @PriceProductGroupId, @PriceProductGroupIdB, @Qty, @Room, @Mounting, @Width, @WidthB, @WidthC, @Drop, @MeshType, @FrameColour, @LayoutCode, @MidrailPosition, @HandleType, @HandleLength, @TripleLock, @BugSeal, @PetType, @PetPosition, @DoorCloser, @AngleType, @AngleLength, @Beading, @JambType, @JambPosition, @FlushBold, @InterlockType, @TopTrack, @TopTrackLength, @BottomTrack, @BottomTrackLength, @Receiver, @ReceiverLength, @SlidingQty, @LinearMetre, @SquareMetre, @SquareMetreB, @SquareMetreC, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@WidthB", widthb)
                        myCmd.Parameters.AddWithValue("@WidthC", widthc)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@MidrailPosition", data.midrailposition)
                        myCmd.Parameters.AddWithValue("@HandleType", data.handletype)
                        myCmd.Parameters.AddWithValue("@HandleLength", handlelength)
                        myCmd.Parameters.AddWithValue("@TripleLock", data.triplelock)
                        myCmd.Parameters.AddWithValue("@BugSeal", data.bugseal)
                        myCmd.Parameters.AddWithValue("@PetType", data.pettype)
                        myCmd.Parameters.AddWithValue("@PetPosition", data.petposition)
                        myCmd.Parameters.AddWithValue("@DoorCloser", data.doorcloser)
                        myCmd.Parameters.AddWithValue("@AngleType", data.angletype)
                        myCmd.Parameters.AddWithValue("@AngleLength", anglelength)
                        myCmd.Parameters.AddWithValue("@Beading", data.beading)
                        myCmd.Parameters.AddWithValue("@JambType", data.jambtype)
                        myCmd.Parameters.AddWithValue("@JambPosition", data.jambposition)
                        myCmd.Parameters.AddWithValue("@FlushBold", data.flushbold)
                        myCmd.Parameters.AddWithValue("@InterlockType", data.interlocktype)
                        myCmd.Parameters.AddWithValue("@TopTrack", data.toptrack)
                        myCmd.Parameters.AddWithValue("@TopTrackLength", toptracklength)
                        myCmd.Parameters.AddWithValue("@BottomTrack", data.bottomtrack)
                        myCmd.Parameters.AddWithValue("@BottomTrackLength", bottomtracklength)
                        myCmd.Parameters.AddWithValue("@Receiver", data.receivertype)
                        myCmd.Parameters.AddWithValue("@ReceiverLength", receiverlength)
                        myCmd.Parameters.AddWithValue("@SlidingQty", data.slidingqty)
                        myCmd.Parameters.AddWithValue("@TotalItems", "1")
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                        myCmd.Parameters.AddWithValue("@SquareMetreC", squareMetreC)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next

            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, PriceProductGroupId=@PriceProductGroupId, PriceProductGroupIdB=@PriceProductGroupIdB, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, WidthB=@WidthB, WidthC=@WidthC, [Drop]=@Drop, MeshType=@MeshType, FrameColour=@FrameColour, LayoutCode=@LayoutCode, MidrailPosition=@MidrailPosition, HandleType=@HandleType, HandleLength=@HandleLength, TripleLock=@TripleLock, BugSeal=@BugSeal, PetType=@PetType, PetPosition=@PetPosition, DoorCloser=@DoorCloser, AngleType=@AngleType, AngleLength=@AngleLength, Beading=@Beading, JambType=@JambType, JambPosition=@JambPosition, FlushBold=@FlushBold, InterlockType=@InterlockType, TopTrack=@TopTrack, TopTrackLength=@TopTrackLength, BottomTrack=@BottomTrack, BottomTrackLength=@BottomTrackLength, Receiver=@Receiver, ReceiverLength=@ReceiverLength, SlidingQty=@SlidingQty, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, SquareMetreB=@SquareMetreB, SquareMetreC=@SquareMetreC, TotalItems=@TotalItems, Notes=@Notes, MarkUp=@MarkUp WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@PriceProductGroupIdB", If(String.IsNullOrEmpty(priceProductGroupB), CType(DBNull.Value, Object), priceProductGroupB))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@WidthB", widthb)
                    myCmd.Parameters.AddWithValue("@WidthC", widthc)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                    myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                    myCmd.Parameters.AddWithValue("@LayoutCode", data.layoutcode)
                    myCmd.Parameters.AddWithValue("@MidrailPosition", data.midrailposition)
                    myCmd.Parameters.AddWithValue("@HandleType", data.handletype)
                    myCmd.Parameters.AddWithValue("@HandleLength", handlelength)
                    myCmd.Parameters.AddWithValue("@TripleLock", data.triplelock)
                    myCmd.Parameters.AddWithValue("@BugSeal", data.bugseal)
                    myCmd.Parameters.AddWithValue("@PetType", data.pettype)
                    myCmd.Parameters.AddWithValue("@PetPosition", data.petposition)
                    myCmd.Parameters.AddWithValue("@DoorCloser", data.doorcloser)
                    myCmd.Parameters.AddWithValue("@AngleType", data.angletype)
                    myCmd.Parameters.AddWithValue("@AngleLength", anglelength)
                    myCmd.Parameters.AddWithValue("@Beading", data.beading)
                    myCmd.Parameters.AddWithValue("@JambType", data.jambtype)
                    myCmd.Parameters.AddWithValue("@JambPosition", data.jambposition)
                    myCmd.Parameters.AddWithValue("@FlushBold", data.flushbold)
                    myCmd.Parameters.AddWithValue("@InterlockType", data.interlocktype)
                    myCmd.Parameters.AddWithValue("@TopTrack", data.toptrack)
                    myCmd.Parameters.AddWithValue("@TopTrackLength", toptracklength)
                    myCmd.Parameters.AddWithValue("@BottomTrack", data.bottomtrack)
                    myCmd.Parameters.AddWithValue("@BottomTrackLength", bottomtracklength)
                    myCmd.Parameters.AddWithValue("@Receiver", data.receivertype)
                    myCmd.Parameters.AddWithValue("@ReceiverLength", receiverlength)
                    myCmd.Parameters.AddWithValue("@SlidingQty", data.slidingqty)
                    myCmd.Parameters.AddWithValue("@TotalItems", "1")
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetreB", squareMetreB)
                    myCmd.Parameters.AddWithValue("@SquareMetreC", squareMetreC)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "PLEASE CONTACT YOUR CUSTOMER SERVICE !"
    End Function

    <WebMethod()>
    Public Shared Function WindowProcess(data As ProccessData) As String
        Dim orderClass As New OrderClass

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim width As Integer
        Dim drop As Integer
        Dim anglelength As Integer
        Dim angleqty As Integer
        Dim swivelqty As Integer
        Dim swivelqtyb As Integer
        Dim springqty As Integer
        Dim topplasticqty As Integer
        Dim markup As Integer

        Dim designName As String = String.Empty
        Dim blindName As String = String.Empty

        If Not String.IsNullOrEmpty(data.designid) Then designName = orderClass.GetDesignName(data.designid)
        If Not String.IsNullOrEmpty(data.blindtype) Then blindName = orderClass.GetBlindName(data.blindtype)

        Dim roleName As String = orderClass.GetUserRoleName(data.loginid)

        If String.IsNullOrEmpty(data.blindtype) Then Return "TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PRODUCT IS REQUIRED !"
        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If

        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        If String.IsNullOrEmpty(data.drop) Then Return "DROP IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR DROP ORDER !"

        If blindName = "Flyscreen" OrElse blindName = "Safety" OrElse blindName = "Security" Then
            If String.IsNullOrEmpty(data.meshtype) Then Return "MESH TYPE IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.framecolour) Then Return "FRAME COLOUR IS REQUIRED !"

        If blindName = "Standard" OrElse blindName = "Flyscreen" OrElse blindName = "Safety" Then
            If String.IsNullOrEmpty(data.brace) Then Return "BRACE / JOINER HEIGHT IS REQUIRED !"
        End If

        If String.IsNullOrEmpty(data.angletype) Then Return "ANGLE TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.anglelength) Then Return "ANGLE LENGTH IS REQUIRED !"
        If Not Integer.TryParse(data.anglelength, anglelength) OrElse anglelength <= 0 Then Return "PLEASE CHECK YOUR ANGLE LENGTH ORDER !"
        If String.IsNullOrEmpty(data.angleqty) Then Return "ANGLE QTY IS REQUIRED !"
        If Not Integer.TryParse(data.angleqty, angleqty) OrElse angleqty <= 0 Then Return "PLEASE CHECK YOUR ANGLE QTY ORDER !"

        If blindName = "Flyscreen" Then
            If String.IsNullOrEmpty(data.porthole) Then Return "SCREEN PORT HOLE IS REQUIRED !"
            If String.IsNullOrEmpty(data.plungerpin) Then Return "PLUNGER PIN IS REQUIRED !"
            If String.IsNullOrEmpty(data.swivelcolour) Then Return "SWIVEL CLIP COLOUR IS REQUIRED !"
            If String.IsNullOrEmpty(data.swivelqty) Then Return "SWIVEL CLIP QTY FOR 1.6MM IS REQUIRED !"
            If Not Integer.TryParse(data.swivelqty, swivelqty) OrElse swivelqty <= 0 Then Return "PLEASE CHECK YOUR SWIVEL CLIP QTY FOR 1.6MM ORDER !"
            If String.IsNullOrEmpty(data.swivelqtyb) Then Return "SWIVEL CLIP QTY FOR 11MM IS REQUIRED !"
            If Not Integer.TryParse(data.swivelqtyb, swivelqtyb) OrElse swivelqtyb <= 0 Then Return "PLEASE CHECK YOUR SWIVEL CLIP QTY FOR 11MM ORDER !"
            If String.IsNullOrEmpty(data.springqty) Then Return "SPRING CLIP QTY IS REQUIRED !"
            If Not Integer.TryParse(data.springqty, springqty) OrElse springqty <= 0 Then Return "PLEASE CHECK YOUR SPRING CLIP QTY ORDER !"
            If String.IsNullOrEmpty(data.topplasticqty) Then Return "TOP CLIP PLASTIC QTY IS REQUIRED !"
            If Not Integer.TryParse(data.topplasticqty, topplasticqty) OrElse topplasticqty <= 0 Then Return "PLEASE CHECK YOUR TOP CLIP PLASTIC QTY ORDER !"
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        If Not blindName = "Flyscreen" Then
            data.porthole = String.Empty
            data.plungerpin = String.Empty
            data.swivelcolour = String.Empty
        End If

        If blindName = "Security" Then
            data.brace = String.Empty
        End If

        Dim linearMetre As Decimal = width / 1000
        Dim squareMetre As Decimal = width * drop / 1000000

        Dim factory As String = String.Empty
        If data.framecolour.Contains("Express") Then factory = "Express"
        If data.framecolour.Contains("Regular") Then factory = "Regular"

        Dim groupName As String = String.Format("{0} {1} {2}", designName, blindName, factory)
        If blindName = "Standard" AndAlso (data.meshtype = "Fibreglass Mesh" OrElse data.meshtype = "Pawproof" OrElse data.meshtype = "SS Mesh") Then
            groupName = String.Format("{0} {1} {2} + Flyscreen Mesh", designName, blindName, factory)
        End If
        Dim priceProductGroup As String = orderClass.GetPriceProductGroupId(groupName, data.designid)

        If data.itemaction = "create" OrElse data.itemaction = "copy" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderClass.GetNewOrderItemId()

                Using thisConn As SqlConnection = New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, ProductId, PriceProductGroupId, Qty, Room, Mounting, Width, [Drop], MeshType, FrameColour, Brace, AngleType, AngleLength, AngleQty, PortHole, PlungerPin, SwivelColour, SwivelQty, SwivelQtyB, SpringQty, TopPlasticQty, LinearMetre, SquareMetre, TotalItems, Notes, MarkUp, Active) VALUES(@Id, @HeaderId, @ProductId, @PriceProductGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @MeshType, @FrameColour, @Brace, @AngleType, @AngleLength, @AngleQty, @PortHole, @PlungerPin, @SwivelColour, @SwivelQty, @SwivelQtyB, @SpringQty, @TopPlasticQty, @LinearMetre, @SquareMetre, @TotalItems, @Notes, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                        myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                        myCmd.Parameters.AddWithValue("@Qty", "1")
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                        myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                        myCmd.Parameters.AddWithValue("@Brace", data.brace)
                        myCmd.Parameters.AddWithValue("@AngleType", data.angletype)
                        myCmd.Parameters.AddWithValue("@AngleLength", anglelength)
                        myCmd.Parameters.AddWithValue("@AngleQty", angleqty)
                        myCmd.Parameters.AddWithValue("@PortHole", data.porthole)
                        myCmd.Parameters.AddWithValue("@PlungerPin", data.plungerpin)
                        myCmd.Parameters.AddWithValue("@SwivelColour", data.swivelcolour)
                        myCmd.Parameters.AddWithValue("@SwivelQty", swivelqty)
                        myCmd.Parameters.AddWithValue("@SwivelQtyB", swivelqtyb)
                        myCmd.Parameters.AddWithValue("@SpringQty", springqty)
                        myCmd.Parameters.AddWithValue("@TopPlasticQty", topplasticqty)
                        myCmd.Parameters.AddWithValue("@TotalItems", "1")
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                orderClass.ResetPriceDetail(data.headerid, itemId)
                orderClass.CalculatePrice(data.headerid, itemId)
                orderClass.FinalCostItem(data.headerid, itemId)

                Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Added"}
                orderClass.Logs(dataLog)
            Next

            Return "Success"
        End If

        If data.itemaction = "edit" OrElse data.itemaction = "view" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, PriceProductGroupId=@PriceProductGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, MeshType=@MeshType, FrameColour=@FrameColour, Brace=@Brace, AngleType=@AngleType, AngleLength=@AngleLength, AngleQty=@AngleQty, PortHole=@PortHole, PlungerPin=@PlungerPin, SwivelColour=@SwivelColour, SwivelQty=@SwivelQty, SwivelQtyB=@SwivelQtyB, SpringQty=@SpringQty, TopPlasticQty=@TopPlasticQty, LinearMetre=@LinearMetre, SquareMetre=@SquareMetre, TotalItems=@TotalItems, Notes=@Notes, MarkUp=@MarkUp, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", data.colourtype)
                    myCmd.Parameters.AddWithValue("@PriceProductGroupId", If(String.IsNullOrEmpty(priceProductGroup), CType(DBNull.Value, Object), priceProductGroup))
                    myCmd.Parameters.AddWithValue("@Qty", "1")
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@MeshType", data.meshtype)
                    myCmd.Parameters.AddWithValue("@FrameColour", data.framecolour)
                    myCmd.Parameters.AddWithValue("@Brace", data.brace)
                    myCmd.Parameters.AddWithValue("@AngleType", data.angletype)
                    myCmd.Parameters.AddWithValue("@AngleLength", anglelength)
                    myCmd.Parameters.AddWithValue("@AngleQty", angleqty)
                    myCmd.Parameters.AddWithValue("@PortHole", data.porthole)
                    myCmd.Parameters.AddWithValue("@PlungerPin", data.plungerpin)
                    myCmd.Parameters.AddWithValue("@SwivelColour", data.swivelcolour)
                    myCmd.Parameters.AddWithValue("@SwivelQty", swivelqty)
                    myCmd.Parameters.AddWithValue("@SwivelQtyB", swivelqtyb)
                    myCmd.Parameters.AddWithValue("@SpringQty", springqty)
                    myCmd.Parameters.AddWithValue("@TopPlasticQty", topplasticqty)
                    myCmd.Parameters.AddWithValue("@TotalItems", "1")
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderClass.ResetPriceDetail(data.headerid, itemId)
            orderClass.CalculatePrice(data.headerid, itemId)
            orderClass.FinalCostItem(data.headerid, itemId)

            Dim dataLog As Object() = {"OrderDetails", itemId, data.loginid, "Order Item Updated"}
            orderClass.Logs(dataLog)

            Return "Success"
        End If

        Return "TEST"
    End Function

    'DETAIL

    <WebMethod()>
    Public Shared Function AluminiumDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignId, Products.BlindId AS BlindType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignId").ToString()
        Dim blindId As String = detailData("BlindType").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .companydetailid = companyDetailId, .tubetype = "0", .controltype = "0", .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim subTypeReq As New JSONList With {.type = "SubType", .blindtype = blindId, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .ColourTypes = ListData(colourReq),
            .Mountings = ListData(mountingReq),
            .SubTypes = ListData(subTypeReq)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function CellularDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()
        Dim fabricId As String = detailData("FabricId").ToString()
        Dim fabricIdB As String = detailData("FabricIdB").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeCS", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim controlReq As New JSONList With {.type = "ControlType", .blindtype = blindId, .tubetype = tubeId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = controlId, .companydetailid = companyDetailId, .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim fabricReq As New JSONList With {.type = "FabricType", .designtype = designId, .companydetailid = companyDetailId, .tubetype = tubeId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReqB As New JSONList With {.type = "FabricColour", .fabrictype = fabricIdB, .companydetailid = companyDetailId, .action = action}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .TubeTypes = ListData(tubeReq),
                .ControlTypes = ListData(controlReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .Fabrics = ListData(fabricReq),
                .FabricColours = ListData(fabricColourReq),
                .FabricColoursB = ListData(fabricColourReqB)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function CurtainDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()
        Dim fabricId As String = detailData("FabricId").ToString()
        Dim fabricIdB As String = detailData("FabricIdB").ToString()

        Dim heading As String = detailData("Heading").ToString()
        Dim headingB As String = detailData("HeadingB").ToString()

        Dim trackType As String = detailData("TrackType").ToString()
        Dim trackTypeB As String = detailData("TrackTypeB").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}

        Dim colourReq As New JSONList With {.type = "ProductName", .blindtype = blindId, .tubetype = "0", .controltype = "0", .companydetailid = companyDetailId, .action = action}

        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}

        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}

        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}

        Dim fabricColourReqB As New JSONList With {.type = "FabricColour", .fabrictype = fabricIdB, .action = action}

        Dim trackTypeReq As New JSONList With {.type = "CurtainTrackType", .customtype = heading, .action = action}

        Dim trackTypeReqB As New JSONList With {.type = "CurtainTrackType", .customtype = headingB, .action = action}

        Dim trackColourReq As New JSONList With {.type = "CurtainTrackColour", .customtype = trackType, .action = action}

        Dim trackColourReqB As New JSONList With {.type = "CurtainTrackColour", .customtype = trackTypeB, .action = action}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .Fabrics = ListData(fabricReq),
                .FabricColours = ListData(fabricColourReq),
                .FabricColoursB = ListData(fabricColourReqB),
                .TrackTypes = ListData(trackTypeReq),
                .TrackTypesB = ListData(trackTypeReqB),
                .TrackColours = ListData(trackColourReq),
                .TrackColoursB = ListData(trackColourReqB)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function DesignDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()
        Dim fabricId As String = detailData("FabricId").ToString()
        Dim fabricIdB As String = detailData("FabricIdB").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim controlReq As New JSONList With {.type = "ControlType", .blindtype = blindId, .tubetype = "0", .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = "0", .controltype = controlId, .companydetailid = companyDetailId, .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}
        Dim chainColourReq As New JSONList With {.type = "ControlColour", .designtype = designId, .controltype = controlId, .companydetailid = companyDetailId, .action = action}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .ControlTypes = ListData(controlReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .Fabrics = ListData(fabricReq),
                .FabricColours = ListData(fabricColourReq),
                .ChainColours = ListData(chainColourReq)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function LineaDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()
        Dim fabricId As String = detailData("FabricId").ToString()
        Dim fabricIdB As String = detailData("FabricIdB").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeCS", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = "0", .companydetailid = companyDetailId}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .TubeTypes = ListData(tubeReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .Fabrics = ListData(fabricReq),
                .FabricColours = ListData(fabricColourReq)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function OutdoorDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()
        Dim fabricId As String = detailData("FabricId").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeCS", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim controlReq As New JSONList With {.type = "ControlType", .blindtype = blindId, .tubetype = "0", .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = "0", .controltype = controlId, .companydetailid = companyDetailId, .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .companydetailid = companyDetailId, .action = action}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .ControlTypes = ListData(controlReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .Fabrics = ListData(fabricReq),
                .FabricColours = ListData(fabricColourReq)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function PanelDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()
        Dim fabricId As String = detailData("FabricId").ToString()
        Dim fabricIdB As String = detailData("FabricIdB").ToString()
        Dim trackType As String = detailData("TrackType").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeCS", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = "0", .companydetailid = companyDetailId, .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim fabricReq As New JSONList With {.type = "FabricType", .designtype = designId, .companydetailid = companyDetailId, .tubetype = tubeId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .companydetailid = companyDetailId, .action = action}
        Dim layoutCodeReq As New JSONList With {.type = "LayoutCodePG", .customtype = trackType, .action = action}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .TubeTypes = ListData(tubeReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .Fabrics = ListData(fabricReq),
                .FabricColours = ListData(fabricColourReq),
                .LayoutCodes = ListData(layoutCodeReq)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function RollerDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()

        Dim blindName As String = detailData("BlindName").ToString()

        Dim fabricId As String = detailData("FabricId").ToString()
        Dim fabricIdB As String = detailData("FabricIdB").ToString()
        Dim fabricIdC As String = detailData("FabricIdC").ToString()
        Dim fabricIdD As String = detailData("FabricIdD").ToString()
        Dim fabricIdE As String = detailData("FabricIdE").ToString()
        Dim fabricIdF As String = detailData("FabricIdF").ToString()

        Dim bottomId As String = detailData("BottomId").ToString()
        Dim bottomIdB As String = detailData("BottomIdB").ToString()
        Dim bottomIdC As String = detailData("BottomIdC").ToString()
        Dim bottomIdD As String = detailData("BottomIdD").ToString()
        Dim bottomIdE As String = detailData("BottomIdE").ToString()
        Dim bottomIdF As String = detailData("BottomIdF").ToString()

        Dim chainId As String = detailData("ChainId").ToString()
        Dim chainIdB As String = detailData("ChainIdB").ToString()
        Dim chainIdC As String = detailData("ChainIdC").ToString()
        Dim chainIdD As String = detailData("ChainIdD").ToString()
        Dim chainIdE As String = detailData("ChainIdE").ToString()
        Dim chainIdF As String = detailData("ChainIdF").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeRoller", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim controlReq As New JSONList With {.type = "ControlType", .blindtype = blindId, .tubetype = tubeId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = controlId, .companydetailid = companyDetailId, .action = action}

        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}

        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}
        Dim fabricColourReqB As New JSONList With {.type = "FabricColour", .fabrictype = fabricIdB, .action = action}
        Dim fabricColourReqC As New JSONList With {.type = "FabricColour", .fabrictype = fabricIdC, .action = action}
        Dim fabricColourReqD As New JSONList With {.type = "FabricColour", .fabrictype = fabricIdD, .action = action}
        Dim fabricColourReqE As New JSONList With {.type = "FabricColour", .fabrictype = fabricIdE, .action = action}
        Dim fabricColourReqF As New JSONList With {.type = "FabricColour", .fabrictype = fabricIdF, .action = action}

        Dim bottomReq As New JSONList With {.type = "BottomType", .designtype = designId, .companydetailid = companyDetailId, .tubetype = tubeId, .action = action}
        Dim bottomColourReq As New JSONList With {.type = "BottomColour", .bottomtype = bottomId, .action = action}
        Dim bottomColourReqB As New JSONList With {.type = "BottomColour", .bottomtype = bottomIdB, .action = action}
        Dim bottomColourReqC As New JSONList With {.type = "BottomColour", .bottomtype = bottomIdC, .action = action}
        Dim bottomColourReqD As New JSONList With {.type = "BottomColour", .bottomtype = bottomIdD, .action = action}
        Dim bottomColourReqE As New JSONList With {.type = "BottomColour", .bottomtype = bottomIdE, .action = action}
        Dim bottomColourReqF As New JSONList With {.type = "BottomColour", .bottomtype = bottomIdF, .action = action}

        Dim chainCustom As String = String.Empty
        If blindName = "Full Cassette" OrElse blindName = "Semi Cassette" Then chainCustom = "Cassette"

        Dim chainReq As New JSONList With {.type = "ControlColour", .designtype = designId, .companydetailid = companyDetailId, .controltype = controlId, .customtype = chainCustom, .action = action}

        Dim stopperReq As New JSONList With {.type = "ChainStopper", .chaincolour = chainId, .action = action}
        Dim stopperReqB As New JSONList With {.type = "ChainStopper", .chaincolour = chainIdB, .action = action}
        Dim stopperReqC As New JSONList With {.type = "ChainStopper", .chaincolour = chainIdC, .action = action}
        Dim stopperReqD As New JSONList With {.type = "ChainStopper", .chaincolour = chainIdD, .action = action}
        Dim stopperReqE As New JSONList With {.type = "ChainStopper", .chaincolour = chainIdE, .action = action}
        Dim stopperReqF As New JSONList With {.type = "ChainStopper", .chaincolour = chainIdF, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .ControlTypes = ListData(controlReq),
            .TubeTypes = ListData(tubeReq),
            .ColourTypes = ListData(colourReq),
            .Mountings = ListData(mountingReq),
            .Fabrics = ListData(fabricReq),
            .FabricColours = ListData(fabricColourReq),
            .FabricColoursB = ListData(fabricColourReqB),
            .FabricColoursC = ListData(fabricColourReqC),
            .FabricColoursD = ListData(fabricColourReqD),
            .FabricColoursE = ListData(fabricColourReqE),
            .FabricColoursF = ListData(fabricColourReqF),
            .Bottoms = ListData(bottomReq),
            .BottomColours = ListData(bottomColourReq),
            .BottomColoursB = ListData(bottomColourReqB),
            .BottomColoursC = ListData(bottomColourReqC),
            .BottomColoursD = ListData(bottomColourReqD),
            .BottomColoursE = ListData(bottomColourReqE),
            .BottomColoursF = ListData(bottomColourReqF),
            .Chains = ListData(chainReq),
            .Stoppers = ListData(stopperReq),
            .StoppersB = ListData(stopperReqB),
            .StoppersC = ListData(stopperReqC),
            .StoppersD = ListData(stopperReqD),
            .StoppersE = ListData(stopperReqE),
            .StoppersF = ListData(stopperReqF)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function RomanDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()

        Dim blindName As String = detailData("BlindName").ToString()

        Dim fabricId As String = detailData("FabricId").ToString()

        Dim chainId As String = detailData("ChainId").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim controlReq As New JSONList With {.type = "ControlType", .blindtype = blindId, .tubetype = tubeId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = controlId, .companydetailid = companyDetailId, .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim fabricReq As New JSONList With {.type = "FabricType", .designtype = designId, .companydetailid = companyDetailId, .tubetype = tubeId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}
        Dim chainReq As New JSONList With {.type = "ControlColour", .designtype = designId, .companydetailid = companyDetailId, .controltype = controlId, .action = action}
        Dim valanceReq As New JSONList With {.type = "ValanceRoman", .controltype = controlId, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .ControlTypes = ListData(controlReq),
            .TubeTypes = ListData(tubeReq),
            .ColourTypes = ListData(colourReq),
            .Mountings = ListData(mountingReq),
            .Fabrics = ListData(fabricReq),
            .FabricColours = ListData(fabricColourReq),
            .Chains = ListData(chainReq),
            .Valances = ListData(valanceReq)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function PelmetDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()

        Dim blindName As String = detailData("BlindName").ToString()

        Dim fabricId As String = detailData("FabricId").ToString()

        Dim chainId As String = detailData("ChainId").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = "0", .companydetailid = companyDetailId, .action = action}

        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}

        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .TubeTypes = ListData(tubeReq),
            .ColourTypes = ListData(colourReq),
            .Mountings = ListData(mountingReq),
            .Fabrics = ListData(fabricReq),
            .FabricColours = ListData(fabricColourReq)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function SaphoraDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()

        Dim blindName As String = detailData("BlindName").ToString()

        Dim fabricId As String = detailData("FabricId").ToString()
        Dim chainId As String = detailData("ChainId").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim controlReq As New JSONList With {.type = "ControlType", .blindtype = blindId, .tubetype = tubeId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = controlId, .companydetailid = companyDetailId, .action = action}

        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}

        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}

        Dim chainReq As New JSONList With {.type = "ControlColour", .designtype = designId, .companydetailid = companyDetailId, .controltype = controlId, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .ControlTypes = ListData(controlReq),
            .TubeTypes = ListData(tubeReq),
            .ColourTypes = ListData(colourReq),
            .Mountings = ListData(mountingReq),
            .Fabrics = ListData(fabricReq),
            .FabricColours = ListData(fabricColourReq),
            .Chains = ListData(chainReq)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function VenetianDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignId, Products.BlindId AS BlindType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignId").ToString()
        Dim blindId As String = detailData("BlindType").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .companydetailid = companyDetailId, .tubetype = "0", .controltype = "0", .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim subTypeReq As New JSONList With {.type = "SubType", .blindtype = blindId, .action = action}
        Dim valanceTypeReq As New JSONList With {.type = "ValanceType", .blindtype = blindId, .action = action}
        Dim valancePositionReq As New JSONList With {.type = "ValancePosition", .blindtype = blindId, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .ColourTypes = ListData(colourReq),
            .Mountings = ListData(mountingReq),
            .SubTypes = ListData(subTypeReq),
            .ValanceTypes = ListData(valanceTypeReq),
            .ValancePositions = ListData(valancePositionReq)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function VerticalDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()
        Dim controlId As String = detailData("ControlType").ToString()

        Dim blindName As String = detailData("BlindName").ToString()

        Dim fabricId As String = detailData("FabricId").ToString()
        Dim chainId As String = detailData("ChainId").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim controlReq As New JSONList With {.type = "ControlType", .blindtype = blindId, .tubetype = tubeId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = controlId, .companydetailid = companyDetailId, .action = action}

        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}

        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}

        Dim chainReq As New JSONList With {.type = "ControlColour", .designtype = designId, .companydetailid = companyDetailId, .controltype = controlId, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .ControlTypes = ListData(controlReq),
            .TubeTypes = ListData(tubeReq),
            .ColourTypes = ListData(colourReq),
            .Mountings = ListData(mountingReq),
            .Fabrics = ListData(fabricReq),
            .FabricColours = ListData(fabricColourReq),
            .Chains = ListData(chainReq)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function DoorDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim tubeId As String = detailData("TubeType").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeCS", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim tubeReq As New JSONList With {.type = "TubeType", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = tubeId, .controltype = "0", .companydetailid = companyDetailId, .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim layoutCodeReq As New JSONList With {.type = "LayoutCodeDoor", .tubetype = tubeId, .action = action}
        Dim meshReq As New JSONList With {.type = "MeshDoor", .blindtype = blindId, .action = action}
        Dim interlockReq As New JSONList With {.type = "InterlockDoor", .tubetype = tubeId, .action = action}
        Dim frameColourReq As New JSONList With {.type = "FrameColourDoor", .blindtype = blindId, .action = action, .companydetailid = companyDetailId}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .TubeTypes = ListData(tubeReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .LayoutCodes = ListData(layoutCodeReq),
                .MeshTypes = ListData(meshReq),
                .Interlocks = ListData(interlockReq),
                .FrameColours = ListData(frameColourReq)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function WindowDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeCS", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = "0", .controltype = "0", .companydetailid = companyDetailId, .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim meshReq As New JSONList With {.type = "MeshWindow", .blindtype = blindId, .action = action}
        Dim frameColourReq As New JSONList With {.type = "FrameColourWindow", .blindtype = blindId, .companydetailid = companyDetailId, .action = action}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .MeshTypes = ListData(meshReq),
                .FrameColours = ListData(frameColourReq)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function SkylineDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()
        Dim mounting As String = detailData("Mounting").ToString()
        Dim frameType As String = detailData("FrameType").ToString()
        Dim frameBottom As String = detailData("FrameBottom").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeCS", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = "0", .controltype = "0", .companydetailid = companyDetailId, .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}
        Dim layoutCodeReq As New JSONList With {.type = "LayoutCodeShutter", .blindtype = blindId, .action = action}
        Dim frameTypeReq As New JSONList With {.type = "FrameTypeShutter", .blindtype = blindId, .customtype = mounting, .action = action}
        Dim leftFrameReq As New JSONList With {.type = "LeftFrameShutter", .customtype = frameType, .action = action}
        Dim rightFrameReq As New JSONList With {.type = "RightFrameShutter", .customtype = frameType, .action = action}
        Dim topFrameReq As New JSONList With {.type = "TopFrameShutter", .customtype = frameType, .action = action}
        Dim bottomFrameReq As New JSONList With {.type = "BottomFrameShutter", .customtype = frameType, .action = action}
        Dim bottomTrackReq As New JSONList With {.type = "BottomTrackShutter", .blindtype = blindId, .customtype = frameBottom, .action = action}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq),
                .LayoutCodes = ListData(layoutCodeReq),
                .FrameTypes = ListData(frameTypeReq),
                .LeftFrames = ListData(leftFrameReq),
                .RightFrames = ListData(rightFrameReq),
                .TopFrames = ListData(topFrameReq),
                .BottomFrames = ListData(bottomFrameReq),
                .BottomTracks = ListData(bottomTrackReq)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function SampleDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType, Blinds.Name AS BlindName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id LEFT JOIN Blinds ON Products.BlindId=Blinds.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()

        Dim blindName As String = detailData("BlindName").ToString()

        Dim fabricId As String = detailData("FabricId").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = "0", .controltype = "0", .companydetailid = companyDetailId, .action = action}

        Dim fabricReq As New JSONList With {.type = "FabricTypeByDesign", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim fabricColourReq As New JSONList With {.type = "FabricColour", .fabrictype = fabricId, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .ColourTypes = ListData(colourReq),
            .Fabrics = ListData(fabricReq),
            .FabricColours = ListData(fabricColourReq)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function PrivacyDetail(itemId As Integer, companyDetailId As String, action As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignId, Products.BlindId AS BlindType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignId").ToString()
        Dim blindId As String = detailData("BlindType").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindType", .designtype = designId, .companydetailid = companyDetailId, .action = action}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .companydetailid = companyDetailId, .tubetype = "0", .controltype = "0", .action = action}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId, .action = action}

        Dim result = New With {
            .ItemData = itemDetail,
            .BlindTypes = ListData(blindReq),
            .ColourTypes = ListData(colourReq),
            .Mountings = ListData(mountingReq)
        }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function EvolveDetail(itemId As Integer, companyDetailId As String) As Object
        Dim orderClass As New OrderClass

        Dim detailData As DataRow = orderClass.GetDataRow("SELECT OrderDetails.*, Products.DesignId AS DesignType, Products.BlindId AS BlindType, Products.TubeType AS TubeType, Products.ControlType AS ControlType FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId=Products.Id WHERE OrderDetails.Id='" & itemId & "'")
        If detailData Is Nothing Then Return Nothing

        Dim designId As String = detailData("DesignType").ToString()
        Dim blindId As String = detailData("BlindType").ToString()

        Dim itemDetail As New Dictionary(Of String, Object)
        For Each col As DataColumn In detailData.Table.Columns
            itemDetail(col.ColumnName) = detailData(col.ColumnName)
        Next

        Dim blindReq As New JSONList With {.type = "BlindTypeCS", .designtype = designId, .companydetailid = companyDetailId}
        Dim colourReq As New JSONList With {.type = "ColourType", .blindtype = blindId, .tubetype = "0", .controltype = "0", .companydetailid = companyDetailId}
        Dim mountingReq As New JSONList With {.type = "Mounting", .blindtype = blindId}

        Dim result = New With {
                .ItemData = itemDetail,
                .BlindTypes = ListData(blindReq),
                .ColourTypes = ListData(colourReq),
                .Mountings = ListData(mountingReq)
            }
        Return result
    End Function

    <WebMethod()>
    Public Shared Function GetLogs(type As String, dataId As String) As List(Of LogDto)
        Dim orderClass As New OrderClass
        Dim dt As DataTable = orderClass.GetDataTable("SELECT * FROM Logs WHERE Type='" & type & "' AND DataId='" & dataId & "' ORDER BY ActionDate ASC")

        Dim result As New List(Of LogDto)

        For Each r As DataRow In dt.Rows
            result.Add(New LogDto With {
            .TextLog = orderClass.GetTextLog(r("Id").ToString())
        })
        Next

        Return result
    End Function

    <WebMethod()>
    Public Shared Function GetCostings(itemId As String, companyId As String) As Object
        Dim orderClass As New OrderClass
        Dim actionClass As New ActionClass

        Dim thisQuery As String = "SELECT *, FORMAT(BuyPrice, 'C', 'en-US') AS BuyPricing, FORMAT(SellPrice, 'C', 'en-US') AS SellPricing FROM OrderCostings WHERE ItemId='" & itemId & "' AND Type<>'Final' AND Number<>0 ORDER BY Number, CASE WHEN Type='Base' THEN 1 WHEN Type='Surcharge' THEN 2 ELSE 3 END ASC"
        If companyId = "3" OrElse companyId = "5" Then
            thisQuery = "SELECT *, FORMAT(BuyPrice, 'C', 'id-ID') AS BuyPricing, FORMAT(SellPrice, 'C', 'id-ID') AS SellPricing FROM OrderCostings WHERE ItemId='" & itemId & "' AND Type<>'Final' AND Number<>0 ORDER BY Number, CASE WHEN Type='Base' THEN 1 WHEN Type='Surcharge' THEN 2 ELSE 3 END ASC"
        End If

        Dim dt = orderClass.GetDataTable(thisQuery)

        Dim list As New List(Of CostingDto)

        For Each r As DataRow In dt.Rows
            list.Add(New CostingDto With {
                .Type = r("Type").ToString(),
                .Description = r("Description").ToString(),
                .BuyPricing = r("BuyPricing").ToString(),
                .SellPricing = r("SellPricing").ToString(),
                .Price = r("SellPricing").ToString()
            })
        Next

        Dim roleId As String = HttpContext.Current.Session("RoleId").ToString()
        Dim levelId As String = HttpContext.Current.Session("LevelId").ToString()

        Return New With {
            .data = list,
            .showType = actionClass.GetActionAccess(roleId, levelId, "Order Detail", "Price Details | Visible Type"),
            .showBuy = actionClass.GetActionAccess(roleId, levelId, "Order Detail", "Price Details | Visible Buy Price"),
            .showSell = actionClass.GetActionAccess(roleId, levelId, "Order Detail", "Price Details | Visible Sell Price"),
            .showPrice = actionClass.GetActionAccess(roleId, levelId, "Order Detail", "Price Details | Visible Price")
        }
    End Function

    <WebMethod()>
    Public Shared Function GetHistoryNote(headerId As String) As List(Of HistoryNoteDto)
        Dim orderClass As New OrderClass

        Dim dt = orderClass.GetDataTable("SELECT OrderInternalNotes.Note, CustomerLogins.FullName, FORMAT(OrderInternalNotes.CreatedDate, 'dd MMM yyyy') AS CreatedDate FROM OrderInternalNotes INNER JOIN CustomerLogins ON OrderInternalNotes.CreatedBy = CustomerLogins.Id WHERE OrderInternalNotes.HeaderId='" & headerId & "' ORDER BY OrderInternalNotes.CreatedDate DESC")

        Dim list As New List(Of HistoryNoteDto)

        For Each r As DataRow In dt.Rows
            list.Add(New HistoryNoteDto With {
                .FullName = r("FullName").ToString(),
                .CreatedDate = r("CreatedDate").ToString(),
                .Note = r("Note").ToString()
            })
        Next

        Return list
    End Function
End Class

Public Class ProccessData
    Public Property headerid As String
    Public Property orderid As String
    Public Property itemaction As String
    Public Property itemid As String
    Public Property customerid As String
    Public Property companyid As String
    Public Property companydetailid As String
    Public Property loginid As String
    Public Property rolename As String
    Public Property designid As String
    Public Property blindtype As String
    Public Property tubetype As String
    Public Property controltype As String
    Public Property colourtype As String
    Public Property subtype As String
    Public Property qty As String
    Public Property qtyblade As String
    Public Property room As String
    Public Property mounting As String
    Public Property width As String
    Public Property widthb As String
    Public Property widthc As String
    Public Property widthd As String
    Public Property widthe As String
    Public Property widthf As String
    Public Property drop As String
    Public Property dropb As String
    Public Property dropc As String
    Public Property dropd As String
    Public Property drope As String
    Public Property dropf As String
    Public Property controlcolour As String
    Public Property controlcolourb As String
    Public Property stackposition As String
    Public Property stackpositionb As String
    Public Property roll As String
    Public Property rollb As String
    Public Property rollc As String
    Public Property rolld As String
    Public Property rolle As String
    Public Property rollf As String
    Public Property controlposition As String
    Public Property controlpositionb As String
    Public Property controlpositionc As String
    Public Property controlpositiond As String
    Public Property controlpositione As String
    Public Property controlpositionf As String
    Public Property tilterposition As String
    Public Property controllength As String
    Public Property controllengthvalue As String
    Public Property controllengthvalue2 As String
    Public Property controllengthb As String
    Public Property controllengthvalueb As String
    Public Property controllengthvalueb2 As String
    Public Property controllengthc As String
    Public Property controllengthvaluec As String
    Public Property controllengthvaluec2 As String
    Public Property controllengthd As String
    Public Property controllengthvalued As String
    Public Property controllengthvalued2 As String
    Public Property controllengthe As String
    Public Property controllengthvaluee As String
    Public Property controllengthvaluee2 As String
    Public Property controllengthf As String
    Public Property controllengthvaluef As String
    Public Property controllengthvaluef2 As String
    Public Property cordlengthvalue As String
    Public Property chainlengthvalue As String
    Public Property wandcolour As String
    Public Property wandlength As String
    Public Property wandlengthvalue As String
    Public Property wandlengthb As String
    Public Property wandlengthvalueb As String
    Public Property layoutcode As String
    Public Property layoutcodecustom As String
    Public Property tracktype As String
    Public Property tracktypeB As String
    Public Property trackcolour As String
    Public Property trackcolourb As String
    Public Property trackdraw As String
    Public Property trackdrawb As String
    Public Property toptrack As String
    Public Property supply As String
    Public Property tassel As String
    Public Property batten As String
    Public Property battenb As String
    Public Property valanceoption As String
    Public Property valancetype As String
    Public Property valancesize As String
    Public Property valancesizevalue As String
    Public Property returnposition As String
    Public Property returnlength As String
    Public Property returnlengthb As String
    Public Property returnlengthvalue As String
    Public Property returnlengthvalueb As String
    Public Property fabrictype As String
    Public Property fabrictypeb As String
    Public Property fabrictypec As String
    Public Property fabrictyped As String
    Public Property fabrictypee As String
    Public Property fabrictypef As String
    Public Property fabriccolour As String
    Public Property fabriccolourb As String
    Public Property fabriccolourc As String
    Public Property fabriccolourd As String
    Public Property fabriccoloure As String
    Public Property fabriccolourf As String
    Public Property remote As String
    Public Property drycontact As String
    Public Property charger As String
    Public Property extensioncable As String
    Public Property chaincolour As String
    Public Property chaincolourb As String
    Public Property chaincolourc As String
    Public Property chaincolourd As String
    Public Property chaincoloure As String
    Public Property chaincolourf As String
    Public Property chainstopper As String
    Public Property chainstopperb As String
    Public Property chainstopperc As String
    Public Property chainstopperd As String
    Public Property chainstoppere As String
    Public Property chainstopperf As String
    Public Property bottomtype As String
    Public Property bottomtypeb As String
    Public Property bottomtypec As String
    Public Property bottomtyped As String
    Public Property bottomtypee As String
    Public Property bottomtypef As String
    Public Property bottomcolour As String
    Public Property bottomcolourb As String
    Public Property bottomcolourc As String
    Public Property bottomcolourd As String
    Public Property bottomcoloure As String
    Public Property bottomcolourf As String
    Public Property bottomoption As String
    Public Property bottomoptionb As String
    Public Property bottomoptionc As String
    Public Property bottomoptiond As String
    Public Property bottomoptione As String
    Public Property bottomoptionf As String
    Public Property springassist As String
    Public Property brackettype As String
    Public Property bracketsize As String
    Public Property bracketextension As String
    Public Property sloping As String
    Public Property isblindin As String
    Public Property fabricinsert As String
    Public Property bottomjoining As String
    Public Property heading As String
    Public Property headingb As String
    Public Property bottomhem As String
    Public Property tieback As String
    Public Property panelqty As String
    Public Property printing As String
    Public Property printingb As String
    Public Property printingc As String
    Public Property printingd As String
    Public Property printinge As String
    Public Property printingf As String
    Public Property printingg As String
    Public Property printingh As String
    Public Property adjusting As String

    'SHUTTER PUNYA
    Public Property louvresize As String
    Public Property louvreposition As String
    Public Property midrailheight1 As String
    Public Property midrailheight2 As String
    Public Property midrailcritical As String
    Public Property joinedpanels As String
    Public Property hingecolour As String
    Public Property semiinside As String
    Public Property customheaderlength As String
    Public Property framecolour As String
    Public Property frametype As String
    Public Property frameleft As String
    Public Property frameright As String
    Public Property frametop As String
    Public Property framebottom As String
    Public Property bottomtracktype As String
    Public Property bottomtrackrecess As String
    Public Property buildout As String
    Public Property buildoutposition As String
    Public Property samesizepanel As String
    Public Property gap1 As String
    Public Property gap2 As String
    Public Property gap3 As String
    Public Property gap4 As String
    Public Property gap5 As String
    Public Property horizontaltpostheight As String
    Public Property horizontaltpost As String
    Public Property tiltrodtype As String
    Public Property tiltrodsplit As String
    Public Property splitheight1 As String
    Public Property splitheight2 As String
    Public Property reversehinged As String
    Public Property pelmetflat As String
    Public Property extrafascia As String
    Public Property hingesloose As String
    Public Property cutout As String
    Public Property specialshape As String
    Public Property templateprovided As String

    ' DOOR PUNYA
    Public Property midrailposition As String
    Public Property handletype As String
    Public Property handlelength As String
    Public Property triplelock As String
    Public Property bugseal As String
    Public Property pettype As String
    Public Property petposition As String
    Public Property doorcloser As String
    Public Property beading As String
    Public Property jambtype As String
    Public Property jambposition As String
    Public Property flushbold As String
    Public Property interlocktype As String
    Public Property toptracklength As String
    Public Property bottomtrack As String
    Public Property bottomtracklength As String
    Public Property receivertype As String
    Public Property receiverlength As String
    Public Property slidingqty As String
    Public Property meshtype As String
    Public Property brace As String
    Public Property angletype As String
    Public Property anglelength As String
    Public Property angleqty As String
    Public Property porthole As String
    Public Property plungerpin As String
    Public Property swivelcolour As String
    Public Property swivelqty As String
    Public Property swivelqtyb As String
    Public Property springqty As String
    Public Property topplasticqty As String
    Public Property notes As String
    Public Property markup As String
End Class

Public Class JSONList
    Public Property type As String
    Public Property customtype As String
    Public Property designtype As String
    Public Property blindtype As String
    Public Property tubetype As String
    Public Property controltype As String
    Public Property fabrictype As String
    Public Property bottomtype As String
    Public Property chaincolour As String
    Public Property companyid As String
    Public Property companydetailid As String
    Public Property action As String
End Class

Public Class LogDto
    Public Property TextLog As String
End Class

Public Class CostingDto
    Public Property Type As String
    Public Property Description As String
    Public Property BuyPricing As String
    Public Property SellPricing As String
    Public Property Price As String
End Class

Public Class HistoryNoteDto
    Public Property FullName As String
    Public Property CreatedDate As String
    Public Property Note As String
End Class