Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Calculation_Edit
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

        If String.IsNullOrEmpty(Request.QueryString("calculationid")) Then
            Response.Redirect("~/setting/price/calculation", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("calculationid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlPriceGroup_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindDesignType(ddlPriceGroup.SelectedValue)
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

                If ddlFormula.SelectedValue = "SQM" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = ""
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = ""
                End If
                If ddlFormula.SelectedValue = "SQM_MIN" Then
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = ""
                End If
                If ddlFormula.SelectedValue = "SQM_ROUND" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = ""
                End If
                If ddlFormula.SelectedValue = "LM" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = ""
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = ""
                End If
                If ddlFormula.SelectedValue = "LM_MIN" Then
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = ""
                End If
                If ddlFormula.SelectedValue = "CUT_LENGTH" Then
                    txtSellMinSize.Text = "" : txtBuyMinSize.Text = ""
                    txtSellMinWidth.Text = "" : txtBuyMinWidth.Text = ""
                    txtSellMinDrop.Text = "" : txtBuyMinDrop.Text = ""
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE PriceCalculations SET Name=@Name, Method=@Method, PriceGroupId=@PriceGroupId, DesignId=@DesignId, Formula=@Formula, SellMinSize=@SellMinSize, BuyMinSize=@BuyMinSize, FactoryMinSize=@FactoryMinSize, SellMinWidth=@SellMinWidth, BuyMinWidth=@BuyMinWidth, FactoryMinWidth=@FactoryMinWidth, SellMinDrop=@SellMinDrop, BuyMinDrop=@BuyMinDrop, FactoryMinDrop=@FactoryMinDrop Status=@Status WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text)
                        thisCmd.Parameters.AddWithValue("@Method", ddlMethod.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@DesignId", If(String.IsNullOrEmpty(ddlDesignType.SelectedValue), CType(DBNull.Value, Object), ddlDesignType.SelectedValue))
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
                        thisCmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue)

                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceCalculations", lblId.Text, Session("LoginId").ToString(), "Price Calculation Updated"}
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

    Protected Sub BindData(calculationid As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceCalculations WHERE Id='" & calculationid & "'")
            If myData Is Nothing Then
                Response.Redirect("~/setting/price/calculation", False)
                Exit Sub
            End If

            BindPriceGroup()
            BindDesignType(myData("PriceGroupId").ToString())
            BindForm(myData("Formula").ToString())

            txtName.Text = myData("Name").ToString()
            ddlPriceGroup.SelectedValue = myData("PriceGroupId").ToString()
            ddlDesignType.SelectedValue = myData("DesignId").ToString()
            ddlMethod.SelectedValue = myData("Method").ToString()
            ddlFormula.SelectedValue = myData("Formula").ToString()

            txtSellMinSize.Text = myData("SellMinSize").ToString()
            txtBuyMinSize.Text = myData("BuyMinSize").ToString()
            txtSellMinWidth.Text = myData("SellMinWidth").ToString()
            txtBuyMinWidth.Text = myData("BuyMinWidth").ToString()
            txtSellMinDrop.Text = myData("SellMinDrop").ToString()
            txtBuyMinDrop.Text = myData("BuyMinDrop").ToString()
            ddlStatus.SelectedValue = myData("Status").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindForm(formula As String)
        Try
            divMinimumSize.Visible = False
            divMinimumWidth.Visible = False
            divMinimumHeight.Visible = False

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

    Protected Sub BindDesignType(priceGroupId As String)
        ddlDesignType.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(priceGroupId) Then
                Dim companyId As String = settingClass.GetItemData("SELECT CompanyId FROM PriceGroups WHERE Id='" & priceGroupId & "'")
                Dim type As String = settingClass.GetItemData("SELECT Type FROM PriceGroups WHERE Id='" & priceGroupId & "'")

                ddlDesignType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Designs CROSS APPLY STRING_SPLIT(CompanyId, ',') AS companyArray WHERE companyArray.VALUE='" & companyId & "' AND Type='" & type & "' AND Active=1")
                ddlDesignType.DataTextField = "Name"
                ddlDesignType.DataValueField = "Id"
                ddlDesignType.DataBind()

                If ddlDesignType.Items.Count > 0 Then
                    ddlDesignType.Items.Insert(0, New ListItem("", ""))
                End If
            End If

        Catch ex As Exception
            ddlDesignType.Items.Clear()
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
