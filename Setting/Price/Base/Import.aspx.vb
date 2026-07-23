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
            Response.Redirect("~/setting/price/base/editable", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindPriceGroup()
            BindMethod(ddlPriceGroup.SelectedValue)
            BindProductGroup(ddlPriceGroup.SelectedValue)
        End If
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindMethod(ddlPriceGroup.SelectedValue)
        BindProductGroup(ddlPriceGroup.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If Not fuFile.HasFile Then
                MessageError(True, "SORRY. PLEASE UPLOAD FILE")
                Exit Sub
            End If

            Dim msg = ImportExcel(fuFile, ddlMethod.SelectedValue, ddlProductGroup.SelectedValue, ddlPriceGroup.SelectedValue, ddlIncludeBuy.SelectedValue)

            If Not String.IsNullOrEmpty(msg) Then
                MessageError(True, msg)
                Exit Sub
            End If

            Response.Redirect("~/setting/price/base", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base", False)
    End Sub

    Protected Function ImportExcel(upload As FileUpload, method As String, productGroupId As Integer, priceGroupId As Integer, includeBuy As String) As String
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
                        Return "NO WORKSHEET WAS FOUND IN THE EXCEL FILE."
                    End If

                    Dim result As String

                    Dim sellSheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Sell", StringComparison.OrdinalIgnoreCase))

                    If sellSheet Is Nothing Then
                        Return "WORKSHEET 'SELL' COULD NOT BE FOUND."
                    End If

                    result = ReadSheet(sellSheet, "Sell", method, productGroupId, priceGroupId, dt, nextId)
                    If result <> "" Then Return result

                    If includeBuy.Equals("Yes", StringComparison.OrdinalIgnoreCase) Then
                        Dim buySheet = package.Workbook.Worksheets.FirstOrDefault(Function(x) x.Name.Equals("Buy", StringComparison.OrdinalIgnoreCase))

                        If buySheet Is Nothing Then
                            Return "WORKSHEET 'BUY' COULD NOT BE FOUND."
                        End If

                        result = ReadSheet(buySheet, "Buy", method, productGroupId, priceGroupId, dt, nextId)
                        If result <> "" Then Return result
                    End If

                End Using

                If dt.Rows.Count = 0 Then
                    Return "NO DATA WAS FOUND TO IMPORT."
                End If

                Using tran As SqlTransaction = thisConn.BeginTransaction()
                    Try
                        Using cmd As New SqlCommand("DELETE FROM PriceBases WHERE Category='Sell' AND Method=@Method AND ProductGroupId=@ProductGroupId AND PriceGroupId=@PriceGroupId", thisConn, tran)
                            cmd.Parameters.AddWithValue("@Method", method)
                            cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId)
                            cmd.Parameters.AddWithValue("@PriceGroupId", priceGroupId)
                            cmd.ExecuteNonQuery()
                        End Using

                        If dt.Select("Category='Buy'").Length > 0 Then
                            Using cmd As New SqlCommand("DELETE FROM PriceBases WHERE Category='Buy' AND Method=@Method AND ProductGroupId=@ProductGroupId AND PriceGroupId=@PriceGroupId", thisConn, tran)
                                cmd.Parameters.AddWithValue("@Method", method)
                                cmd.Parameters.AddWithValue("@ProductGroupId", productGroupId)
                                cmd.Parameters.AddWithValue("@PriceGroupId", priceGroupId)
                                cmd.ExecuteNonQuery()
                            End Using
                        End If

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

    Private Function ReadSheet(ws As ExcelWorksheet, category As String, method As String, productGroupId As Integer, priceGroupId As Integer, dt As DataTable, ByRef nextId As Integer) As String
        If ws Is Nothing Then Return ""

        If ws.Dimension Is Nothing Then Return ""

        Dim lastRow As Integer = ws.Dimension.End.Row
        Dim lastCol As Integer = ws.Dimension.End.Column

        For r As Integer = 2 To lastRow
            If String.IsNullOrWhiteSpace(ws.Cells(r, 1).Text) Then Continue For

            Dim height As Integer
            If Not Integer.TryParse(ws.Cells(r, 1).Text.Trim(), height) Then
                Return category & " : Height pada baris " & r & " bukan angka."
            End If

            For c As Integer = 2 To lastCol
                Dim width As Integer
                If Not Integer.TryParse(ws.Cells(1, c).Text.Trim(), width) Then
                    Return category & " : Width pada kolom " & c & " bukan angka."
                End If

                Dim price As Decimal
                If Not Decimal.TryParse(ws.Cells(r, c).Text.Trim(), price) Then
                    Return category & " : Price pada baris " & r & ", kolom " & c & " bukan angka."
                End If

                price = Decimal.Round(price, 2, MidpointRounding.AwayFromZero)

                dt.Rows.Add(nextId, category, method, productGroupId, priceGroupId, height, width, price, DBNull.Value)
                nextId += 1
            Next
        Next
        Return ""
    End Function

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups WHERE Type='Blinds' ORDER BY Id ASC")
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

    Protected Sub BindMethod(priceGroupId As String)
        ddlMethod.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(priceGroupId) Then
                If {"1", "9", "17", "11"}.Contains(priceGroupId) Then
                    ddlMethod.Items.Add(New ListItem("Cost", "Cost"))
                End If
                'If {"6", "7", "17", "11"}.Contains(priceGroupId) Then
                '    ddlMethod.Items.Add(New ListItem("Cost", "Cost"))
                'End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub BindProductGroup(priceGroupId As String)
        ddlProductGroup.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(priceGroupId) Then
                Dim companyId As String = settingClass.GetItemData("SELECT CompanyId FROM PriceGroups WHERE Id='" & priceGroupId & "'")

                ddlProductGroup.DataSource = settingClass.GetDataTable("SELECT PPG.Id, PPG.Name FROM PriceProductGroups PPG WHERE EXISTS (SELECT 1 FROM STRING_SPLIT(PPG.CompanyDetailId, ',') S INNER JOIN CompanyDetails CD ON TRY_CAST(S.value AS INT) = CD.Id WHERE CD.CompanyId = '" & companyId & "');")
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
