   M S B A M L   `   `   `  ����@  <DB_UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:DB_UIDB_UI  5   	       DB_UI.OrderDetailsView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   %localclr-namespace:DB_UI     5   	     � Title$  OrderDetailsView��5   	   $	��450��6"   $	��800��6/   .��5	      �  � ColumnDefinitions 5
   
   �� 5      6!   �� 5      6!   �� 5      6!    � RowDefinitions 5   
   �� 5      6   �� 5      6   .+�5   
   �� -   5        $��Delete��5      $��1��6"   $	��Top=�6/   
�������$��Center=�6_   $��30��6|   $	��118��6�   $��0,57,0,0q�6�   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ OrderDetailsGrid��5      $��1��6-   $��2��6=   $��2��6Q   $
��Left=�6b   $	��350��6}   $	��Top=�6�   $	��492��6�   $��	10,42,0,0q�6�   �� 5   
   -   5         ��Name$ 	AmountTxt��5      $��0��6&   $��Right=�63   $��Center=�6P    �� TextWrapping$
 Wrap=�6l   $	��153��6�   $��26��6�   �� 5   
   $��Skaits��6   $
��Left=�6!   $��Center=�6=   $��60��6X   $��40,0,0,0q�6c   �� 5   
   -   5         ��Name$ cboProducts��5      $��0��6)   $��Right=�66   $��25��6R   $	��Top=�6^   $	��153��6v   $��2��6�   $��0,149,266,0q�6�   �� 5   
   $��Prece��6   $
��Left=�6    $��0��6;   $	��Top=�6I   $��60��6a   $��
40,148,0,0q�6l   �� 5   
   -   5        $��	Pievienot��5      
�������$	��100��6:   $��30��6F   $	��Top=�6R   $
��Left=�6j   $��
30,217,0,0q�6�   $��2��6�   �� 5   
   -   5        $��	Rediģēt��5      
�������$	��100��67   $��30��6C   $	��Top=�6P   $��Right=�6h   $��2��6�   $��0,217,266,0q�6�   $��2��6�   5      5      