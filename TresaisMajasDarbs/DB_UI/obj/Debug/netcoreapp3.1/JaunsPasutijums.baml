   M S B A M L   `   `   `  ����	  DB_UI    DB_UI.JaunsPasutijums   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  JaunsPasutijums��5   	   $	��450��6!   $	��800��6.   .��5	      �  � ColumnDefinitions 5
   
   �� 5      6!   �� 5      6!   �� 5      6!    � RowDefinitions 5   
   �� 5      6   �� 5      6   �� 5      6   .+�5   
   �� $��Jauns pasūtījums��6   $��Center=�6+   $��20(�6O   $
��Left=�6]   $��50��6x   $��0��6�   $��1��6�   $��Center=�6�   $	��256��6�   �� 5   
   $��Pasūtījuma numurs��6   $
��Left=�6,   $
��Left=�6O   $��1��6j   $��1��6w   $��35��6�   $	��Top=�6�   $	��130��6�   �� 5   
   -   5         ��Name$ PasutNum��5      $��35��6$   $��1��60   $��1��6=   $��Right=�6M    �� TextWrapping$
 Wrap=�6j   $	��Top=�6~   $	��150��6�   �� 5   
   $��Pasūtījuma datums��6   $
��Left=�6-   $��30��6I   $��1��6U   $��1��6b   $��Bottom=�6r   $	��180��6�   (  "System.Windows.Controls.DatePicker  5   
   -   5          Name$ 	PasutDate��5      $��30��6(   $	��150��64   $��Bottom=�6@   $��Right=�6\   $��1��6x   $��1��6�   �� 5   
   $��Pasūtītājs��6   $��Right=�6%   $
��Left=�6I   $��35��6d   $��1��6p   $	��Top=�6}   $	��112��6�   $��2��6�   $��
266,44,0,0q�6�   �� 5   
   -   5         ��Name$ cboPasutitajs��5      $��30��6*   $	��150��66   $	��Top=�6B   $��1��6Z   $��Right=�6g   $��1��6�   $��0,44,0,0q�6�   �� 5   
   -   5         ��Name$ PievienotPasutijumu��5      $��Izveidot pasūtījumu��6.   $��30��6L   $	��170��6X   $	��Top=�6d   ���	#FFA5CDDC$��2��6�   $��1��6�   $��
48,20,48,0q�6�   �� 5   
   $��State��6   $
��Left=�6!   $��Right=�6<   $��30��6_   $��1��6k   $	��Top=�6x   $	��100��6�   $��2��6�   $��
266,79,0,0q�6�   �� 5   
   -   5        $ cboState��5      $��30��6&   $	��150��62   $��Bottom=�6>   $��Right=�6Y   $��1��6v   $��1��6�   $��0,0,0,35q�6�   5       5!      