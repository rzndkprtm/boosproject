Imports System.Data
Imports System.Data.SqlClient
Imports OfficeOpenXml

Partial Class Setting_Price_Base_Import
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/base", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindPriceGroup()
            BindProductGroup(ddlPriceGroup.SelectedValue)
            bindBuyPrice(ddlPriceGroup.SelectedValue)
        End If
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindProductGroup(ddlPriceGroup.SelectedValue)
        bindBuyPrice(ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub btnSubmitAdd_Click(sender As Object, e As EventArgs)
        Process("Add")
    End Sub

    Protected Sub btnSubmitFinish_Click(sender As Object, e As EventArgs)
        Process()
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base", False)
    End Sub

    Protected Sub Process(Optional action As String = "")
        MessageError(False, String.Empty)
        Try
            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlUploadType.SelectedValue = "" Then
                MessageError(True, "METHOD IS REQUIRED !")
                Exit Sub
            End If
            If ddlProductGroup.SelectedValue = "" Then
                MessageError(True, "PRODUCT GROUP IS REQUIRED !")
                Exit Sub
            End If
            If Not fuFile.HasFile Then
                MessageError(True, "SORRY. PLEASE UPLOAD FILE !")
                Exit Sub
            End If

            Dim uploadType As String = ddlUploadType.SelectedValue
            If ddlUploadType.SelectedValue = "" Then
                uploadType = "Sell"
            End If

            Dim msg = ImportExcel(fuFile, "Cost", ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue, uploadType, ddlBackup.SelectedValue)

            If Not String.IsNullOrEmpty(msg) Then
                MessageError(True, msg)
                Exit Sub
            End If

            Dim url As String = "~/setting/price/base"
            If action = "Add" Then url = "~/setting/price/base/import"
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function ImportExcel(upload As FileUpload, method As String, productGroupId As Integer, priceGroupId As Integer, uploadType As String, backupData As String) As String
        Try
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial

            Dim dt As New DataTable

            dt.Columns.Add("Id", GetType(Integer))
            dt.Columns.Add("Category", GetType(String))
            dt.Columns.Add("Method", GetType(String))
            dt.Columns.Add("ProductGroupId", GetType(Integer))
            dt.Columns.Add("PriceGroupId", GetType(Integer))
            dt.Columns.Add("Height", GetType(Integer))
            dt.Columns.Add("Width", GetType(Integer))
            dt.Columns.Add("Price", GetType(Decimal))
            dt.Columns.Add("Conditional", GetType(String))

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Dim nextId As Integer
                Using cmd As New SqlCommand("SELECT ISNULL(MAX(Id),0)+1 FROM PriceBases", thisConn)
                    nextId = CInt(cmd.ExecuteScalar())
                End Using

                Using package As New ExcelPackage(upload.PostedFile.InputStream)
                    If package.Workbook.Worksheets.Count = 0 Then
                        Return "NO WORKSHEET WAS FOUND IN THE EXCEL FILE !"
                    End If

                    Dim result As String = ""

                    Select Case uploadType.Trim().ToUpper()
                        Case "SELL"
                            Dim sellSheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Sell", StringComparison.OrdinalIgnoreCase))
                            If sellSheet Is Nothing Then
                                Return "WORKSHEET 'SELL' COULD NOT BE FOUND !"
                            End If
                            result = ReadSheet(sellSheet, "Sell", method, productGroupId, priceGroupId, dt, nextId)
                            If result <> "" Then
                                Return result
                            End If
                        Case "BUY"
                            Dim buySheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Buy", StringComparison.OrdinalIgnoreCase))

                            If buySheet Is Nothing Then
                                Return "WORKSHEET 'BUY' COULD NOT BE FOUND !"
                            End If

                            result = ReadSheet(buySheet, "Buy", method, productGroupId, priceGroupId, dt, nextId)

                            If result <> "" Then
                                Return result
                            End If
                        Case "FACTORY"
                            Dim factorySheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Factory", StringComparison.OrdinalIgnoreCase))

                            If factorySheet Is Nothing Then
                                Return "WORKSHEET 'FACTORY' COULD NOT BE FOUND !"
                            End If

                            result = ReadSheet(factorySheet, "Factory", method, productGroupId, priceGroupId, dt, nextId)

                            If result <> "" Then
                                Return result
                            End If
                        Case "COMPLETE"
                            Dim sellSheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Sell", StringComparison.OrdinalIgnoreCase))
                            If sellSheet Is Nothing Then
                                Return "WORKSHEET 'SELL' COULD NOT BE FOUND !"
                            End If
                            result = ReadSheet(sellSheet, "Sell", method, productGroupId, priceGroupId, dt, nextId)
                            If result <> "" Then
                                Return result
                            End If

                            Dim buySheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Buy", StringComparison.OrdinalIgnoreCase))
                            If buySheet Is Nothing Then
                                Return "WORKSHEET 'BUY' COULD NOT BE FOUND !"
                            End If
                            result = ReadSheet(buySheet, "Buy", method, productGroupId, priceGroupId, dt, nextId)
                            If result <> "" Then
                                Return result
                            End If

                            Dim factorySheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Factory", StringComparison.OrdinalIgnoreCase))
                            If factorySheet Is Nothing Then
                                Return "WORKSHEET 'FACTORY' COULD NOT BE FOUND !"
                            End If
                            result = ReadSheet(factorySheet, "Factory", method, productGroupId, priceGroupId, dt, nextId)
                            If result <> "" Then
                                Return result
                            End If
                        Case "SELL (BUY & FACTORY AUTO)"
                            Dim sellSheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Sell", StringComparison.OrdinalIgnoreCase))

                            If sellSheet Is Nothing Then
                                Return "WORKSHEET 'SELL' COULD NOT BE FOUND !"
                            End If

                            result = ReadSheet(sellSheet, "Sell", method, productGroupId, priceGroupId, dt, nextId)

                            If result <> "" Then
                                Return result
                            End If

                            AddBuyFromMaster(dt, productGroupId, priceGroupId, nextId, thisConn)
                        Case Else
                            Return "INVALID UPLOAD TYPE !"
                    End Select

                    If dt.Rows.Count = 0 Then
                        Return "NO DATA WAS FOUND TO IMPORT !"
                    End If
                End Using

                If backupData = "Yes" Then
                    Dim newTable As String = "PriceBases_Backup" & "_" & Session("RoleName").ToString() & "_" & DateTime.Now.ToString("yyyyMMdd_HHmmss")
                    Using cmd As New SqlCommand("SELECT * INTO [dbo].[" & newTable & "] FROM [dbo].[PriceBases]",
                    thisConn)
                        cmd.ExecuteNonQuery()
                    End Using
                End If

                Using tran As SqlTransaction = thisConn.BeginTransaction()
                    Try
                        Dim deleteCategories As New List(Of String)
                        Select Case uploadType.Trim().ToUpper()
                            Case "SELL"
                                deleteCategories.Add("Sell")
                            Case "BUY"
                                deleteCategories.Add("Buy")
                            Case "FACTORY"
                                deleteCategories.Add("Factory")
                            Case "COMPLETE"
                                deleteCategories.Add("Sell")
                                deleteCategories.Add("Buy")
                                deleteCategories.Add("Factory")
                            Case "SELL (BUY & FACTORY AUTO)"
                                deleteCategories.Add("Sell")
                                deleteCategories.Add("Buy")
                                deleteCategories.Add("Factory")
                        End Select

                        Dim sql As String = "DELETE FROM PriceBases WHERE Method=@Method AND ProductGroupId=@ProductGroupId AND PriceGroupId=@PriceGroupId AND Category IN (" & String.Join(",", deleteCategories.Select(Function(x) "'" & x & "'")) & ")"

                        Using cmd As New SqlCommand(sql, thisConn, tran)
                            cmd.Parameters.AddWithValue("@Method", method)
                            cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId)
                            cmd.Parameters.AddWithValue("@PriceGroupId", priceGroupId)
                            cmd.ExecuteNonQuery()
                        End Using

                        Using bulk As New SqlBulkCopy(thisConn, SqlBulkCopyOptions.Default, tran)
                            bulk.DestinationTableName = "PriceBases"
                            bulk.BatchSize = 1000
                            bulk.BulkCopyTimeout = 300

                            bulk.ColumnMappings.Add("Id", "Id")
                            bulk.ColumnMappings.Add("Category", "Category")
                            bulk.ColumnMappings.Add("Method", "Method")
                            bulk.ColumnMappings.Add("ProductGroupId", "ProductGroupId")
                            bulk.ColumnMappings.Add("PriceGroupId", "PriceGroupId")
                            bulk.ColumnMappings.Add("Height", "Height")
                            bulk.ColumnMappings.Add("Width", "Width")
                            bulk.ColumnMappings.Add("Price", "Price")
                            bulk.ColumnMappings.Add("Conditional", "Conditional")

                            bulk.WriteToServer(dt)
                        End Using

                        If uploadType.Trim().ToUpper() = "BUY" Then
                            CopyBuyToNoMaster(productGroupId, priceGroupId, method, thisConn, tran)
                        End If

                        If uploadType.Trim().ToUpper() = "FACTORY" Then
                            CopyFactoryToNoMaster(productGroupId, priceGroupId, method, thisConn, tran)
                        End If

                        tran.Commit()
                    Catch ex As Exception
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            Return ""
        Catch ex As Exception
            Return ex.Message
        End Try
    End Function

    Protected Function ReadSheet(ws As ExcelWorksheet, category As String, method As String, productGroupId As Integer, priceGroupId As Integer, dt As DataTable, ByRef nextId As Integer) As String
        Try
            If ws Is Nothing Then Return ""

            If ws.Dimension Is Nothing Then Return ""

            Dim lastRow As Integer = ws.Dimension.End.Row
            Dim lastCol As Integer = ws.Dimension.End.Column

            For r As Integer = 2 To lastRow
                If String.IsNullOrWhiteSpace(ws.Cells(r, 1).Text) Then Continue For

                Dim height As Integer
                If Not Integer.TryParse(ws.Cells(r, 1).Text.Trim(), height) Then
                    Return category & ": The height value on row " & r & " is not a valid number."
                End If

                For c As Integer = 2 To lastCol
                    Dim width As Integer
                    If Not Integer.TryParse(ws.Cells(1, c).Text.Trim(), width) Then
                        Return category & ": The width value in column " & c & " is not a valid number."
                    End If

                    Dim price As Decimal
                    If Not Decimal.TryParse(ws.Cells(r, c).Text.Trim(), price) Then
                        Return category & ": The price value on row " & r & ", column " & c & " is not a valid number."
                    End If

                    price = Decimal.Round(price, 4, MidpointRounding.AwayFromZero)

                    dt.Rows.Add(nextId, category, method, productGroupId, priceGroupId, height, width, price, DBNull.Value)
                    nextId += 1
                Next
            Next

            Return ""
        Catch ex As Exception
            Return "An unexpected error occurred while reading the Excel worksheet."
        End Try
    End Function

    Protected Sub AddBuyFromMaster(dt As DataTable, productGroupId As Integer, priceGroupId As Integer, ByRef nextId As Integer, conn As SqlConnection)
        Try
            Dim masterPriceGroupId As Integer = 0
            Using cmd As New SqlCommand("SELECT TOP 1 PG2.Id FROM PriceGroups PG1 INNER JOIN PriceGroups PG2 ON PG2.CompanyId = PG1.CompanyId AND PG2.Type = PG1.Type AND PG2.Master = 'Yes' WHERE PG1.Id = @PriceGroupId", conn)
                cmd.Parameters.AddWithValue("@PriceGroupId", priceGroupId)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    masterPriceGroupId = CInt(result)
                End If
            End Using

            If masterPriceGroupId = 0 Then
                Exit Sub
            End If

            Using cmd As New SqlCommand("SELECT Method, ProductGroupId, Height, Width, Price, Conditional FROM PriceBases WHERE Category = 'Buy' AND PriceGroupId = @MasterPriceGroupId AND ProductGroupId = @ProductGroupId", conn)
                cmd.Parameters.AddWithValue("@MasterPriceGroupId", masterPriceGroupId)
                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId)
                Using rd As SqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        Dim row As DataRow = dt.NewRow()

                        row("Id") = nextId
                        row("Category") = "Buy"
                        row("Method") = rd("Method")
                        row("ProductGroupId") = rd("ProductGroupId")
                        row("PriceGroupId") = priceGroupId
                        row("Height") = rd("Height")
                        row("Width") = rd("Width")
                        row("Price") = rd("Price")
                        row("Conditional") = rd("Conditional")
                        dt.Rows.Add(row)
                        nextId += 1
                    End While
                End Using
            End Using
        Catch
            Throw
        End Try
    End Sub

    Protected Sub CopyBuyToNoMaster(productGroupId As Integer, masterPriceGroupId As Integer, method As String, conn As SqlConnection, tran As SqlTransaction)
        Try
            Dim isMaster As Boolean = False

            Using cmd As New SqlCommand("SELECT CASE WHEN Master='Yes' THEN 1 ELSE 0 END FROM PriceGroups WHERE Id=@PriceGroupId", conn, tran)
                cmd.Parameters.AddWithValue("@PriceGroupId", masterPriceGroupId)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    isMaster = Convert.ToBoolean(result)
                End If
            End Using

            If Not isMaster Then
                Throw New Exception("BUY CAN ONLY BE UPLOADED TO MASTER PRICE GROUP.")
            End If
            Dim companyId As Integer
            Dim priceGroupType As String

            Using cmd As New SqlCommand("SELECT CompanyId, Type FROM PriceGroups WHERE Id=@PriceGroupId", conn, tran)
                cmd.Parameters.AddWithValue("@PriceGroupId", masterPriceGroupId)
                Using rd As SqlDataReader = cmd.ExecuteReader()
                    If Not rd.Read() Then
                        Throw New Exception("MASTER PRICE GROUP NOT FOUND.")
                    End If

                    companyId = Convert.ToInt32(rd("CompanyId"))
                    priceGroupType = rd("Type").ToString()
                End Using
            End Using

            Dim priceGroupIds As New List(Of Integer)

            Using cmd As New SqlCommand("SELECT Id FROM PriceGroups WHERE CompanyId=@CompanyId AND Type=@Type AND Master='No'", conn, tran)
                cmd.Parameters.AddWithValue("@CompanyId", companyId)
                cmd.Parameters.AddWithValue("@Type", priceGroupType)

                Using rd As SqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        priceGroupIds.Add(Convert.ToInt32(rd("Id")))
                    End While
                End Using
            End Using

            For Each targetPriceGroupId As Integer In priceGroupIds
                Using cmd As New SqlCommand("DELETE FROM PriceBases WHERE Method=@Method AND ProductGroupId=@ProductGroupId AND PriceGroupId = @PriceGroupId AND Category='Buy'", conn, tran)
                    cmd.Parameters.AddWithValue("@Method", method)
                    cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId)
                    cmd.Parameters.AddWithValue("@PriceGroupId", targetPriceGroupId)
                    cmd.ExecuteNonQuery()
                End Using

                Using cmd As New SqlCommand("INSERT INTO PriceBases (Id, Category, Method, ProductGroupId, PriceGroupId, Height, Width, Price, Conditional) SELECT ISNULL((SELECT MAX(Id) FROM PriceBases), 0) + ROW_NUMBER() OVER (ORDER BY Height, Width), Category, Method, ProductGroupId, @TargetPriceGroupId, Height, Width, Price, Conditional FROM PriceBases WHERE Category='Buy' AND Method=@Method AND ProductGroupId=@ProductGroupId AND PriceGroupId=@MasterPriceGroupId", conn, tran)
                    cmd.Parameters.AddWithValue("@TargetPriceGroupId", targetPriceGroupId)
                    cmd.Parameters.AddWithValue("@Method", method)
                    cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId)
                    cmd.Parameters.AddWithValue("@MasterPriceGroupId", masterPriceGroupId)

                    cmd.ExecuteNonQuery()
                End Using
            Next
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Protected Sub CopyFactoryToNoMaster(productGroupId As Integer, masterPriceGroupId As Integer, method As String, conn As SqlConnection, tran As SqlTransaction)
        Try
            Dim isMaster As Boolean = False

            Using cmd As New SqlCommand("SELECT CASE WHEN Master='Yes' THEN 1 ELSE 0 END FROM PriceGroups WHERE Id=@PriceGroupId", conn, tran)
                cmd.Parameters.AddWithValue("@PriceGroupId", masterPriceGroupId)
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso result IsNot DBNull.Value Then
                    isMaster = Convert.ToBoolean(result)
                End If
            End Using

            If Not isMaster Then
                Throw New Exception("FACTORY CAN ONLY BE UPLOADED TO MASTER PRICE GROUP.")
            End If
            Dim companyId As Integer
            Dim priceGroupType As String

            Using cmd As New SqlCommand("SELECT CompanyId, Type FROM PriceGroups WHERE Id=@PriceGroupId", conn, tran)
                cmd.Parameters.AddWithValue("@PriceGroupId", masterPriceGroupId)
                Using rd As SqlDataReader = cmd.ExecuteReader()
                    If Not rd.Read() Then
                        Throw New Exception("MASTER PRICE GROUP NOT FOUND.")
                    End If

                    companyId = Convert.ToInt32(rd("CompanyId"))
                    priceGroupType = rd("Type").ToString()
                End Using
            End Using

            Dim priceGroupIds As New List(Of Integer)

            Using cmd As New SqlCommand("SELECT Id FROM PriceGroups WHERE CompanyId=@CompanyId AND Type=@Type AND Master='No'", conn, tran)
                cmd.Parameters.AddWithValue("@CompanyId", companyId)
                cmd.Parameters.AddWithValue("@Type", priceGroupType)

                Using rd As SqlDataReader = cmd.ExecuteReader()
                    While rd.Read()
                        priceGroupIds.Add(Convert.ToInt32(rd("Id")))
                    End While
                End Using
            End Using

            For Each targetPriceGroupId As Integer In priceGroupIds
                Using cmd As New SqlCommand("DELETE FROM PriceBases WHERE Method=@Method AND ProductGroupId=@ProductGroupId AND PriceGroupId = @PriceGroupId AND Category='Factory'", conn, tran)
                    cmd.Parameters.AddWithValue("@Method", method)
                    cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId)
                    cmd.Parameters.AddWithValue("@PriceGroupId", targetPriceGroupId)
                    cmd.ExecuteNonQuery()
                End Using

                Using cmd As New SqlCommand("INSERT INTO PriceBases (Id, Category, Method, ProductGroupId, PriceGroupId, Height, Width, Price, Conditional) SELECT ISNULL((SELECT MAX(Id) FROM PriceBases), 0) + ROW_NUMBER() OVER (ORDER BY Height, Width), Category, Method, ProductGroupId, @TargetPriceGroupId, Height, Width, Price, Conditional FROM PriceBases WHERE Category='Factory' AND Method=@Method AND ProductGroupId=@ProductGroupId AND PriceGroupId=@MasterPriceGroupId", conn, tran)
                    cmd.Parameters.AddWithValue("@TargetPriceGroupId", targetPriceGroupId)
                    cmd.Parameters.AddWithValue("@Method", method)
                    cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId)
                    cmd.Parameters.AddWithValue("@MasterPriceGroupId", masterPriceGroupId)

                    cmd.ExecuteNonQuery()
                End Using
            Next
        Catch ex As Exception
            Throw
        End Try
    End Sub

    Protected Sub bindBuyPrice(priceGroupId As String)
        ddlUploadType.Items.Clear()
        Try
            Dim master As String = settingClass.GetItemData("SELECT [Master] FROM PriceGroups WHERE Id='" & priceGroupId & "'")
            If master = "Yes" Then
                ddlUploadType.Items.Add(New ListItem("", ""))
                ddlUploadType.Items.Add(New ListItem("Sell Only", "Sell"))
                ddlUploadType.Items.Add(New ListItem("Buy Only", "Buy"))
                ddlUploadType.Items.Add(New ListItem("Factory Only", "Factory"))
                ddlUploadType.Items.Add(New ListItem("Sell, Buy & Factory", "Complete"))
            ElseIf master = "No" Then
                ddlUploadType.Items.Add(New ListItem("Sell Only (Buy & Factory Auto)", "Sell & Factory (Buy Auto)"))
            Else
                ddlUploadType.Items.Add(New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlUploadType.Items.Clear()
        End Try
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Type='Blinds' AND (Status='Active' OR Status='Inactive') ORDER BY Id ASC")
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlPriceGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub BindProductGroup(priceGroupId As String)
        ddlProductGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(priceGroupId) Then
                Dim query As String = "SELECT PriceProductGroups.Id, PriceProductGroups.Name FROM PriceProductGroups CROSS APPLY STRING_SPLIT(PriceGroupId, ',') AS thisArray WHERE thisArray.VALUE='" & priceGroupId & "'"

                ddlProductGroup.DataSource = settingClass.GetDataTable(query)
                ddlProductGroup.DataTextField = "Name"
                ddlProductGroup.DataValueField = "Id"
                ddlProductGroup.DataBind()

                If ddlProductGroup.Items.Count > 0 Then
                    ddlProductGroup.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlProductGroup.Items.Clear()
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
