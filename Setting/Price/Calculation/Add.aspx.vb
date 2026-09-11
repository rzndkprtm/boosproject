Imports System.Data.SqlClient

Partial Class Setting_Price_Calculation_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/calculation", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindPriceGroup()
            BindDataName(ddlDataType.SelectedValue)

            BindForm(ddlFormula.SelectedValue)
        End If
    End Sub

    Protected Sub ddlDataType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDataName(ddlDataType.SelectedValue)
    End Sub

    Protected Sub ddlFormula_SelectedIndexChanged(sender As Object, e As EventArgs)
        BindForm(ddlFormula.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "CALCULATION NAME IS REQUIRED !")
                Exit Sub
            End If
            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If
            If ddlMethod.SelectedValue = "" Then
                MessageError(True, "METHOD IS REQUIRED !")
                Exit Sub
            End If
            If ddlFormula.SelectedValue = "" Then
                MessageError(True, "FORMULA IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceCalculations ORDER BY Id DESC")

                If ddlFormula.SelectedValue = "COST" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = "" : txtFactoryMinSize.Text = ""
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = "" : txtFactoryMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = "" : txtFactoryMinDrop.Text = ""
                    txtSellFormula.Text = "" : txtBuyFormula.Text = "" : txtFactoryFormula.Text = ""
                End If
                If ddlFormula.SelectedValue = "SQM" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = "" : txtFactoryMinSize.Text = ""
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = "" : txtFactoryMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = "" : txtFactoryMinDrop.Text = ""
                    txtSellFormula.Text = "" : txtBuyFormula.Text = "" : txtFactoryFormula.Text = ""
                End If
                If ddlFormula.SelectedValue = "SQM_MIN" Then
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = "" : txtFactoryMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = "" : txtFactoryMinDrop.Text = ""
                    txtSellFormula.Text = "" : txtBuyFormula.Text = "" : txtFactoryFormula.Text = ""
                End If
                If ddlFormula.SelectedValue = "SQM_ROUND" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = "" : txtFactoryMinSize.Text = ""
                    txtSellFormula.Text = "" : txtBuyFormula.Text = "" : txtFactoryFormula.Text = ""
                End If
                If ddlFormula.SelectedValue = "LM" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = "" : txtFactoryMinSize.Text = ""
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = "" : txtFactoryMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = "" : txtFactoryMinDrop.Text = ""
                    txtSellFormula.Text = "" : txtBuyFormula.Text = "" : txtFactoryFormula.Text = ""
                End If
                If ddlFormula.SelectedValue = "LM_MIN" Then
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = "" : txtFactoryMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = "" : txtFactoryMinDrop.Text = ""
                    txtSellFormula.Text = "" : txtBuyFormula.Text = "" : txtFactoryFormula.Text = ""
                End If
                If ddlFormula.SelectedValue = "CUT_LENGTH" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = "" : txtFactoryMinSize.Text = ""
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = "" : txtFactoryMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = "" : txtFactoryMinDrop.Text = ""
                    txtSellFormula.Text = "" : txtBuyFormula.Text = "" : txtFactoryFormula.Text = ""
                End If
                If ddlFormula.SelectedValue = "FEET" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = "" : txtFactoryMinSize.Text = ""
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = "" : txtFactoryMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = "" : txtFactoryMinDrop.Text = ""
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO PriceCalculations VALUES (@Id, @Name, @Method, @PriceGroupId, @DataType, @DataId, @Formula, @SellMinSize, @BuyMinSize, @FactoryMinSize, @SellMinWidth, @BuyMinWidth, @FactoryMinWidth, @SellMinDrop, @BuyMinDrop, @FactoryMinDrop, @SellFormula, @BuyFormula, @FactoryFormula, @Status)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text)
                        thisCmd.Parameters.AddWithValue("@Method", ddlMethod.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@DataType", If(String.IsNullOrEmpty(ddlDataType.SelectedValue), CType(DBNull.Value, Object), ddlDataType.SelectedValue))
                        thisCmd.Parameters.AddWithValue("@DataId", If(String.IsNullOrEmpty(ddlDataId.SelectedValue), CType(DBNull.Value, Object), ddlDataId.SelectedValue))
                        thisCmd.Parameters.AddWithValue("@Formula", ddlFormula.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@SellMinSize", If(String.IsNullOrEmpty(txtSellMinSize.Text), CType(DBNull.Value, Object), txtSellMinSize.Text))
                        thisCmd.Parameters.AddWithValue("@BuyMinSize", If(String.IsNullOrEmpty(txtBuyMinSize.Text), CType(DBNull.Value, Object), txtBuyMinSize.Text))
                        thisCmd.Parameters.AddWithValue("@FactoryMinSize", If(String.IsNullOrEmpty(txtFactoryMinSize.Text), CType(DBNull.Value, Object), txtFactoryMinSize.Text))
                        thisCmd.Parameters.AddWithValue("@SellMinWidth", If(String.IsNullOrEmpty(txtSellMinWidth.Text), CType(DBNull.Value, Object), txtSellMinWidth.Text))
                        thisCmd.Parameters.AddWithValue("@BuyMinWidth", If(String.IsNullOrEmpty(txtBuyMinWidth.Text), CType(DBNull.Value, Object), txtBuyMinWidth.Text))
                        thisCmd.Parameters.AddWithValue("@FactoryMinWidth", If(String.IsNullOrEmpty(txtFactoryMinWidth.Text), CType(DBNull.Value, Object), txtFactoryMinWidth.Text))
                        thisCmd.Parameters.AddWithValue("@SellMinDrop", If(String.IsNullOrEmpty(txtSellMinDrop.Text), CType(DBNull.Value, Object), txtSellMinDrop.Text))
                        thisCmd.Parameters.AddWithValue("@BuyMinDrop", If(String.IsNullOrEmpty(txtBuyMinDrop.Text), CType(DBNull.Value, Object), txtBuyMinDrop.Text))
                        thisCmd.Parameters.AddWithValue("@FactoryMinDrop", If(String.IsNullOrEmpty(txtFactoryMinDrop.Text), CType(DBNull.Value, Object), txtFactoryMinDrop.Text))
                        thisCmd.Parameters.AddWithValue("@SellFormula", If(String.IsNullOrEmpty(txtSellFormula.Text), CType(DBNull.Value, Object), txtSellFormula.Text))
                        thisCmd.Parameters.AddWithValue("@BuyFormula", If(String.IsNullOrEmpty(txtBuyFormula.Text), CType(DBNull.Value, Object), txtBuyFormula.Text))
                        thisCmd.Parameters.AddWithValue("@FactoryFormula", If(String.IsNullOrEmpty(txtFactoryFormula.Text), CType(DBNull.Value, Object), txtFactoryFormula.Text))
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)

                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceCalculations", thisId, Session("LoginId").ToString(), "Price Calculation Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/price/calculation", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/calculation", False)
    End Sub

    Protected Sub BindForm(formula As String)
        Try
            divMinimumSize.Visible = False
            divMinimumWidth.Visible = False
            divMinimumHeight.Visible = False
            divCustomFormula.Visible = False

            If formula = "SQM_MIN" Then
                divMinimumSize.Visible = True
            End If
            If formula = "SQM_ROUND" Then
                divMinimumWidth.Visible = True
                divMinimumHeight.Visible = True
            End If
            If formula = "LM_MIN" Then
                divMinimumSize.Visible = True
            End If
            If formula = "FEET" Then
                divCustomFormula.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups")
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

    Protected Sub BindDataName(dataType As String)
        ddlDataId.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(dataType) Then
                Dim thisString As String = String.Empty
                If dataType = "Designs" Then
                    thisString = "SELECT Id, Name FROM Designs WHERE Active=1"
                End If
                If dataType = "Blinds" Then
                    thisString = "SELECT Blinds.Id, Blinds.Name + ' [' + Designs.Name + ']' AS Name FROM Blinds LEFT JOIN Designs ON Blinds.DesignId=Designs.Id WHERE Blinds.Active=1"
                End If

                ddlDataId.DataSource = settingClass.GetDataTable(thisString)
                ddlDataId.DataTextField = "Name"
                ddlDataId.DataValueField = "Id"
                ddlDataId.DataBind()

                If ddlDataId.Items.Count > 0 Then
                    ddlDataId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlDataId.Items.Clear()
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
