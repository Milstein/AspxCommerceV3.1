﻿/*
AspxCommerce® - http://www.aspxcommerce.com
Copyright (c) 2011-2014 by AspxCommerce

Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OF OTHER DEALINGS IN THE SOFTWARE. 
*/



using System.Runtime.Serialization;

namespace AspxCommerce.RewardPoints
{
    public class RewardRuleListInfo
    {
        private System.Nullable<int> _rewardRuleID;
        private string _rewardRuleType;
   

        [DataMember]
        public System.Nullable<int> RewardRuleID
        {
            get
            {
                return this._rewardRuleID;
            }
            set
            {
                if (this._rewardRuleID != value)
                {
                    this._rewardRuleID = value;
                }
            }
        }
        [DataMember]
        public string RewardRuleType
        {
            get
            {
                return this._rewardRuleType;
            }
            set
            {
                if (this._rewardRuleType != value)
                {
                    this._rewardRuleType = value;
                }
            }
        }
        
        
    }
}
