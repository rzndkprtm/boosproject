Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Base_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Private Property PriceBaseRows As DataTable
        Get
            If ViewState("PriceBaseRows") Is Nothing Then
                Dim dt As New DataTable()

                dt.Columns.Add("ProductGroupId")
                dt.Columns.Add("Height")
                dt.Columns.Add("Width")
                dt.Columns.Add("Price")

                ViewState("PriceBaseRows") = dt
            End If
            Return DirectCast(ViewState("PriceBaseRows"), DataTable)
        End Get
        Set(value As DataTable)
            ViewState("PriceBaseRows") = value
        End Set
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/base", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindPriceGroup()
            AddRow()

            BindRepeater()
        End If
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        PriceBaseRows = Nothing
        AddRow()
        BindRepeater()
    End Sub

    Protected Sub rptPriceBase_ItemDataBound(sender As Object, e As RepeaterItemEventArgs)
        Try
            If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
                Dim ddl As DropDownList = DirectCast(e.Item.FindControl("ddlProductGroupRow"), DropDownList)

                BindProductGroupRow(ddl, ddlPriceGroup.SelectedValue)

                Dim index As Integer = e.Item.ItemIndex
                Dim row As DataRow = PriceBaseRows.Rows(index)

                If row("ProductGroupId").ToString <> "" Then
                    ddl.SelectedValue = row("ProductGroupId").ToString
                End If

                DirectCast(e.Item.FindControl("txtHeightRow"), TextBox).Text = row("Height").ToString
                DirectCast(e.Item.FindControl("txtWidthRow"), TextBox).Text = row("Width").ToString
                DirectCast(e.Item.FindControl("txtPriceRow"), TextBox).Text = row("Price").ToString
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub rptPriceBase_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        Try
            If e.CommandName = "Delete" Then
                SaveRows()

                Dim index As Integer = Convert.ToInt32(e.CommandArgument)

                Dim dt As DataTable = PriceBaseRows

                dt.Rows.RemoveAt(index)

                If dt.Rows.Count = 0 Then
                    AddRow()
                End If
                PriceBaseRows = dt
                BindRepeater()
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnAddRow_Click(sender As Object, e As EventArgs)
        SaveRows()
        AddRow()
        BindRepeater()
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlCategory.SelectedValue = "" Then
                MessageError(True, "CATEGORY IS REQUIRED !")
                Exit Sub
            End If
            If ddlMethod.SelectedValue = "" Then
                MessageError(True, "METHOD IS REQUIRED !")
                Exit Sub
            End If

            SaveRows()

            If PriceBaseRows.Rows.Count = 0 Then
                MessageError(True, "AT LEAST ONE ROW IS REQUIRED !")
                Exit Sub
            End If

            For i As Integer = 0 To PriceBaseRows.Rows.Count - 1
                Dim row As DataRow = PriceBaseRows.Rows(i)

                If row("ProductGroupId").ToString() = "" Then
                    MessageError(True, "PRODUCT GROUP IS REQUIRED ON ROW " & (i + 1).ToString() & " !")
                    Exit Sub
                End If

                If row("Height").ToString() = "" Then
                    MessageError(True, "HEIGHT IS REQUIRED ON ROW " & (i + 1).ToString() & " !")
                    Exit Sub
                End If

                If row("Width").ToString() = "" Then
                    MessageError(True, "WIDTH IS REQUIRED ON ROW " & (i + 1).ToString() & " !")
                    Exit Sub
                End If

                If row("Price").ToString() = "" Then
                    MessageError(True, "PRICE IS REQUIRED ON ROW " & (i + 1).ToString() & " !")
                    Exit Sub
                End If
            Next

            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()

                Using thisTransaction As SqlTransaction = thisConn.BeginTransaction()
                    Try
                        Dim nextId As Integer
                        Using idCmd As New SqlCommand("SELECT ISNULL(MAX(Id), 0) + 1 FROM PriceBases", thisConn, thisTransaction)
                            nextId = Convert.ToInt32(idCmd.ExecuteScalar())
                        End Using

                        For Each row As DataRow In PriceBaseRows.Rows
                            Dim thisId As Integer = nextId

                            Using thisCmd As New SqlCommand("INSERT INTO PriceBases VALUES(@Id, @Category, @Method, @ProductGroupId, @PriceGroupId, @Height, @Width, @Price, NULL)", thisConn, thisTransaction)
                                thisCmd.Parameters.AddWithValue("@Id", thisId)
                                thisCmd.Parameters.AddWithValue("@Category", ddlCategory.SelectedValue)
                                thisCmd.Parameters.AddWithValue("@Method", ddlMethod.SelectedValue)
                                thisCmd.Parameters.AddWithValue("@ProductGroupId", row("ProductGroupId").ToString())
                                thisCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                                thisCmd.Parameters.AddWithValue("@Height", row("Height").ToString())
                                thisCmd.Parameters.AddWithValue("@Width", row("Width").ToString())
                                thisCmd.Parameters.AddWithValue("@Price", row("Price").ToString())

                                thisCmd.ExecuteNonQuery()
                            End Using

                            Dim dataLog As Object() = {"PriceBases", thisId, Session("LoginId").ToString(), "Price Base Updated"}
                            settingClass.Logs(dataLog)

                            nextId += 1
                        Next
                        thisTransaction.Commit()
                    Catch
                        thisTransaction.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            PriceBaseRows = Nothing
            Response.Redirect("~/setting/price/base", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/base", False)
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM PriceGroups WHERE Status='Active' ORDER BY Name ASC")
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

    Protected Sub BindProductGroupRow(ddl As DropDownList, priceGroupId As String)
        ddl.Items.Clear()
        Try
            If String.IsNullOrEmpty(priceGroupId) Then
                Exit Sub
            End If

            Dim query As String = "SELECT DISTINCT PriceProductGroups.Id, PriceProductGroups.Name FROM PriceProductGroups CROSS APPLY STRING_SPLIT(PriceGroupId, ',') AS thisArray WHERE thisArray.VALUE = @PriceGroupId ORDER BY PriceProductGroups.Name"
            Using conn As New SqlConnection(myConn)
                Using cmd As New SqlCommand(query, conn)
                    cmd.Parameters.AddWithValue("@PriceGroupId", priceGroupId)

                    Using adapter As New SqlDataAdapter(cmd)
                        Dim dt As New DataTable()
                        adapter.Fill(dt)
                        ddl.DataSource = dt
                        ddl.DataTextField = "Name"
                        ddl.DataValueField = "Id"
                        ddl.DataBind()
                    End Using
                End Using
            End Using

            ddl.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddl.Items.Clear()
        End Try
    End Sub

    Protected Sub AddRow()
        Try
            Dim dt As DataTable = PriceBaseRows

            Dim row As DataRow = dt.NewRow()
            row("ProductGroupId") = ""
            row("Height") = ""
            row("Width") = ""
            row("Price") = ""

            dt.Rows.Add(row)

            PriceBaseRows = dt
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindRepeater()
        Try
            rptPriceBase.DataSource = PriceBaseRows
            rptPriceBase.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub SaveRows()
        Try
            Dim dt As DataTable = PriceBaseRows

            For i As Integer = 0 To rptPriceBase.Items.Count - 1
                Dim item As RepeaterItem = rptPriceBase.Items(i)

                dt.Rows(i)("ProductGroupId") = DirectCast(item.FindControl("ddlProductGroupRow"), DropDownList).SelectedValue
                dt.Rows(i)("Height") = DirectCast(item.FindControl("txtHeightRow"), TextBox).Text
                dt.Rows(i)("Width") = DirectCast(item.FindControl("txtWidthRow"), TextBox).Text
                dt.Rows(i)("Price") = DirectCast(item.FindControl("txtPriceRow"), TextBox).Text
            Next
            PriceBaseRows = dt
        Catch ex As Exception
            MessageError(True, ex.ToString())
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
