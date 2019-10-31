﻿using System;
using System.Collections.Generic;

namespace AspNetCore.MVC.RESTful.Parameters
{
    public class OrderByPropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; private set; }
        public bool Reverse { get; private set; }

        public OrderByPropertyMappingValue(IEnumerable<string> destinationProperties,
            bool reverse = false)
        {
            DestinationProperties = destinationProperties
                                    ?? throw new ArgumentNullException(nameof(destinationProperties));
            Reverse = reverse;
        }
    }
}