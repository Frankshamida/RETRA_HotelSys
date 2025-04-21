
SET IDENTITY_INSERT [RETRA_HotelSys_Db].[dbo].[RoomFeatures] ON;

  INSERT INTO [RETRA_HotelSys_Db].[dbo].[RoomFeatures]
    ([AmenityId], [Name], [Description], [IconClass], [DefaultAdditionalCost], [FeatureName])
VALUES
    (1, 'Free WiFi', 'High-speed wireless internet access', 'fa fa-wifi', 0.00, 'Internet'),
    (2, 'Premium WiFi', 'Ultra-fast WiFi for streaming & work', 'fa fa-wifi', 10.00, 'Internet'),
    (3, 'Breakfast Buffet', 'Full breakfast with hot & cold options', 'fa fa-coffee', 25.00, 'Dining'),
    (4, 'Air Conditioning', 'Climate control system', 'fa fa-snowflake', 0.00, 'Comfort'),
    (5, 'Heating', 'Adjustable room heating', 'fa fa-thermometer-half', 0.00, 'Comfort'),
    (6, 'Smart TV', '40" LED TV with Netflix & streaming', 'fa fa-tv', 0.00, 'Entertainment'),
    (7, 'Mini Bar', 'Stocked with drinks & snacks', 'fa fa-glass', 50.00, 'Refreshments'),
    (8, 'In-Room Safe', 'Digital safe for valuables', 'fa fa-lock', 5.00, 'Security'),
    (9, '24/7 Room Service', 'On-demand food delivery', 'fa fa-bell', 15.00, 'Service'),
    (10, 'Jacuzzi Tub', 'Private whirlpool bathtub', 'fa fa-bath', 30.00, 'Luxury'),
    (11, 'Pet-Friendly', 'Pet bed & bowl provided', 'fa fa-paw', 20.00, 'Pets'),
    (12, 'Parking (Daily)', 'Secure underground parking', 'fa fa-car', 12.00, 'Transportation'),
    (13, 'Balcony', 'Private outdoor seating area', 'fa fa-home', 15.00, 'View'),
    (14, 'Ocean View', 'Direct sea-facing room', 'fa fa-binoculars', 40.00, 'View'),
    (15, 'Work Desk', 'Ergonomic workspace', 'fa fa-laptop', 0.00, 'Business'),
    (16, 'Meeting Room Access', 'Business meeting facilities', 'fa fa-briefcase', 75.00, 'Business'),
    (17, 'Laundry Service', 'Same-day dry cleaning', 'fa fa-tshirt', 20.00, 'Service'),
    (18, 'Fitness Center', '24-hour gym access', 'fa fa-dumbbell', 0.00, 'Wellness'),
    (19, 'Spa Access', 'Complimentary spa entry', 'fa fa-spa', 0.00, 'Wellness'),
    (20, 'Pool Access', 'Infinity pool & lounge', 'fa fa-swimming-pool', 0.00, 'Recreation'),
    (21, 'Private Pool', 'Exclusive pool for suite guests', 'fa fa-water', 100.00, 'Luxury'),
    (22, 'Kitchenette', 'Microwave, fridge & sink', 'fa fa-utensils', 25.00, 'Convenience'),
    (23, 'Crib (On Request)', 'Baby crib available', 'fa fa-baby', 0.00, 'Family'),
    (24, 'Early Check-In', 'Check in from 10 AM', 'fa fa-clock', 20.00, 'Service'),
    (25, 'Late Check-Out', 'Check out until 2 PM', 'fa fa-clock', 20.00, 'Service');

	
SET IDENTITY_INSERT [RETRA_HotelSys_Db].[dbo].[RoomFeatures] OFF;