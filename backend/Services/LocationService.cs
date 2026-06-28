using System.Text.Json.Serialization;
using ClimateAdvisor.Api.Models;

namespace ClimateAdvisor.Api.Services;

public class LocationService : ILocationService
{
    private readonly HttpClient _http;
    private static List<CountryInfo>? _cachedCountries;
    private static readonly object _lock = new();

    public LocationService(HttpClient http)
    {
        _http = http;
    }

    public Task<List<CountryInfo>> GetCountriesAsync()
    {
        if (_cachedCountries != null)
            return Task.FromResult(_cachedCountries);

        lock (_lock)
        {
            _cachedCountries ??= BuildCountryList();
        }

        return Task.FromResult(_cachedCountries);
    }

    public async Task<string?> DetectCountryFromIpAsync(string ipAddress, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(ipAddress) || IsPrivateIp(ipAddress))
            return null;

        try
        {
            var url = $"http://ip-api.com/json/{ipAddress}?fields=status,countryCode";
            var response = await _http.GetFromJsonAsync<IpApiResponse>(url, ct);

            if (response?.Status == "success" && !string.IsNullOrEmpty(response.CountryCode))
                return response.CountryCode.ToUpperInvariant();
        }
        catch { }

        return null;
    }

    private static bool IsPrivateIp(string ip)
    {
        if (ip == "::1" || ip == "127.0.0.1") return true;
        if (ip.StartsWith("10.") || ip.StartsWith("192.168.")) return true;
        if (ip.StartsWith("172."))
        {
            var parts = ip.Split('.');
            if (parts.Length > 1 && int.TryParse(parts[1], out var second))
                return second >= 16 && second <= 31;
        }
        return false;
    }

    private static List<CountryInfo> BuildCountryList() => new()
    {
        new() { Code = "AF", Name = "Afghanistan", Latitude = 33.93911, Longitude = 67.709953, Region = "Asia" },
        new() { Code = "AL", Name = "Albania", Latitude = 41.153332, Longitude = 20.168331, Region = "Europe" },
        new() { Code = "DZ", Name = "Algeria", Latitude = 28.033886, Longitude = 1.659626, Region = "Africa" },
        new() { Code = "AD", Name = "Andorra", Latitude = 42.506285, Longitude = 1.521801, Region = "Europe" },
        new() { Code = "AO", Name = "Angola", Latitude = -11.202692, Longitude = 17.873887, Region = "Africa" },
        new() { Code = "AG", Name = "Antigua and Barbuda", Latitude = 17.060816, Longitude = -61.796428, Region = "Americas" },
        new() { Code = "AR", Name = "Argentina", Latitude = -38.416097, Longitude = -63.616672, Region = "Americas" },
        new() { Code = "AM", Name = "Armenia", Latitude = 40.069099, Longitude = 45.038189, Region = "Asia" },
        new() { Code = "AU", Name = "Australia", Latitude = -25.274398, Longitude = 133.775136, Region = "Oceania" },
        new() { Code = "AT", Name = "Austria", Latitude = 47.516231, Longitude = 14.550072, Region = "Europe" },
        new() { Code = "AZ", Name = "Azerbaijan", Latitude = 40.143105, Longitude = 47.576927, Region = "Asia" },
        new() { Code = "BS", Name = "Bahamas", Latitude = 25.03428, Longitude = -77.39628, Region = "Americas" },
        new() { Code = "BH", Name = "Bahrain", Latitude = 25.930414, Longitude = 50.637772, Region = "Asia" },
        new() { Code = "BD", Name = "Bangladesh", Latitude = 23.684994, Longitude = 90.356331, Region = "Asia" },
        new() { Code = "BB", Name = "Barbados", Latitude = 13.193887, Longitude = -59.543198, Region = "Americas" },
        new() { Code = "BY", Name = "Belarus", Latitude = 53.709807, Longitude = 27.953389, Region = "Europe" },
        new() { Code = "BE", Name = "Belgium", Latitude = 50.503887, Longitude = 4.469936, Region = "Europe" },
        new() { Code = "BZ", Name = "Belize", Latitude = 17.189877, Longitude = -88.49765, Region = "Americas" },
        new() { Code = "BJ", Name = "Benin", Latitude = 9.30769, Longitude = 2.315834, Region = "Africa" },
        new() { Code = "BT", Name = "Bhutan", Latitude = 27.514162, Longitude = 90.433601, Region = "Asia" },
        new() { Code = "BO", Name = "Bolivia", Latitude = -16.290154, Longitude = -63.588653, Region = "Americas" },
        new() { Code = "BA", Name = "Bosnia and Herzegovina", Latitude = 43.915886, Longitude = 17.679076, Region = "Europe" },
        new() { Code = "BW", Name = "Botswana", Latitude = -22.328474, Longitude = 24.684866, Region = "Africa" },
        new() { Code = "BR", Name = "Brazil", Latitude = -14.235004, Longitude = -51.92528, Region = "Americas" },
        new() { Code = "BN", Name = "Brunei", Latitude = 4.535277, Longitude = 114.727669, Region = "Asia" },
        new() { Code = "BG", Name = "Bulgaria", Latitude = 42.733883, Longitude = 25.48583, Region = "Europe" },
        new() { Code = "BF", Name = "Burkina Faso", Latitude = 12.238333, Longitude = -1.561593, Region = "Africa" },
        new() { Code = "BI", Name = "Burundi", Latitude = -3.373056, Longitude = 29.918886, Region = "Africa" },
        new() { Code = "CV", Name = "Cabo Verde", Latitude = 16.5388, Longitude = -23.0418, Region = "Africa" },
        new() { Code = "KH", Name = "Cambodia", Latitude = 12.565679, Longitude = 104.990963, Region = "Asia" },
        new() { Code = "CM", Name = "Cameroon", Latitude = 7.369722, Longitude = 12.354722, Region = "Africa" },
        new() { Code = "CA", Name = "Canada", Latitude = 56.130366, Longitude = -106.346771, Region = "Americas" },
        new() { Code = "CF", Name = "Central African Republic", Latitude = 6.611111, Longitude = 20.939444, Region = "Africa" },
        new() { Code = "TD", Name = "Chad", Latitude = 15.454166, Longitude = 18.732207, Region = "Africa" },
        new() { Code = "CL", Name = "Chile", Latitude = -35.675147, Longitude = -71.542969, Region = "Americas" },
        new() { Code = "CN", Name = "China", Latitude = 35.86166, Longitude = 104.195397, Region = "Asia" },
        new() { Code = "CO", Name = "Colombia", Latitude = 4.570868, Longitude = -74.297333, Region = "Americas" },
        new() { Code = "KM", Name = "Comoros", Latitude = -11.875001, Longitude = 43.872219, Region = "Africa" },
        new() { Code = "CG", Name = "Congo", Latitude = -0.228021, Longitude = 15.827659, Region = "Africa" },
        new() { Code = "CD", Name = "Congo (DRC)", Latitude = -4.038333, Longitude = 21.758664, Region = "Africa" },
        new() { Code = "CR", Name = "Costa Rica", Latitude = 9.748917, Longitude = -83.753428, Region = "Americas" },
        new() { Code = "CI", Name = "Côte d'Ivoire", Latitude = 7.539989, Longitude = -5.54708, Region = "Africa" },
        new() { Code = "HR", Name = "Croatia", Latitude = 45.1, Longitude = 15.2, Region = "Europe" },
        new() { Code = "CU", Name = "Cuba", Latitude = 21.521757, Longitude = -77.781167, Region = "Americas" },
        new() { Code = "CY", Name = "Cyprus", Latitude = 35.126413, Longitude = 33.429859, Region = "Asia" },
        new() { Code = "CZ", Name = "Czech Republic", Latitude = 49.817492, Longitude = 15.472962, Region = "Europe" },
        new() { Code = "DK", Name = "Denmark", Latitude = 56.26392, Longitude = 9.501785, Region = "Europe" },
        new() { Code = "DJ", Name = "Djibouti", Latitude = 11.825138, Longitude = 42.590275, Region = "Africa" },
        new() { Code = "DM", Name = "Dominica", Latitude = 15.414999, Longitude = -61.370976, Region = "Americas" },
        new() { Code = "DO", Name = "Dominican Republic", Latitude = 18.735693, Longitude = -70.162651, Region = "Americas" },
        new() { Code = "EC", Name = "Ecuador", Latitude = -1.831239, Longitude = -78.183406, Region = "Americas" },
        new() { Code = "EG", Name = "Egypt", Latitude = 26.820553, Longitude = 30.802498, Region = "Africa" },
        new() { Code = "SV", Name = "El Salvador", Latitude = 13.794185, Longitude = -88.89653, Region = "Americas" },
        new() { Code = "GQ", Name = "Equatorial Guinea", Latitude = 1.650801, Longitude = 10.267895, Region = "Africa" },
        new() { Code = "ER", Name = "Eritrea", Latitude = 15.179384, Longitude = 39.782334, Region = "Africa" },
        new() { Code = "EE", Name = "Estonia", Latitude = 58.595272, Longitude = 25.013607, Region = "Europe" },
        new() { Code = "SZ", Name = "Eswatini", Latitude = -26.522503, Longitude = 31.465866, Region = "Africa" },
        new() { Code = "ET", Name = "Ethiopia", Latitude = 9.145, Longitude = 40.489673, Region = "Africa" },
        new() { Code = "FJ", Name = "Fiji", Latitude = -17.713371, Longitude = 178.065032, Region = "Oceania" },
        new() { Code = "FI", Name = "Finland", Latitude = 61.92411, Longitude = 25.748151, Region = "Europe" },
        new() { Code = "FR", Name = "France", Latitude = 46.603354, Longitude = 1.888334, Region = "Europe" },
        new() { Code = "GA", Name = "Gabon", Latitude = -0.803689, Longitude = 11.609444, Region = "Africa" },
        new() { Code = "GM", Name = "Gambia", Latitude = 13.443182, Longitude = -15.310139, Region = "Africa" },
        new() { Code = "GE", Name = "Georgia", Latitude = 42.315407, Longitude = 43.356892, Region = "Asia" },
        new() { Code = "DE", Name = "Germany", Latitude = 51.165691, Longitude = 10.451526, Region = "Europe" },
        new() { Code = "GH", Name = "Ghana", Latitude = 7.946527, Longitude = -1.023194, Region = "Africa" },
        new() { Code = "GR", Name = "Greece", Latitude = 39.074208, Longitude = 21.824312, Region = "Europe" },
        new() { Code = "GD", Name = "Grenada", Latitude = 12.262776, Longitude = -61.604171, Region = "Americas" },
        new() { Code = "GT", Name = "Guatemala", Latitude = 15.783471, Longitude = -90.230759, Region = "Americas" },
        new() { Code = "GN", Name = "Guinea", Latitude = 9.945587, Longitude = -9.696645, Region = "Africa" },
        new() { Code = "GW", Name = "Guinea-Bissau", Latitude = 11.803749, Longitude = -15.180413, Region = "Africa" },
        new() { Code = "GY", Name = "Guyana", Latitude = 4.860416, Longitude = -58.93018, Region = "Americas" },
        new() { Code = "HT", Name = "Haiti", Latitude = 18.971187, Longitude = -72.285215, Region = "Americas" },
        new() { Code = "HN", Name = "Honduras", Latitude = 15.199999, Longitude = -86.241905, Region = "Americas" },
        new() { Code = "HU", Name = "Hungary", Latitude = 47.162494, Longitude = 19.503304, Region = "Europe" },
        new() { Code = "IS", Name = "Iceland", Latitude = 64.963051, Longitude = -19.020835, Region = "Europe" },
        new() { Code = "IN", Name = "India", Latitude = 20.593684, Longitude = 78.96288, Region = "Asia" },
        new() { Code = "ID", Name = "Indonesia", Latitude = -0.789275, Longitude = 113.921327, Region = "Asia" },
        new() { Code = "IR", Name = "Iran", Latitude = 32.427908, Longitude = 53.688046, Region = "Asia" },
        new() { Code = "IQ", Name = "Iraq", Latitude = 33.223191, Longitude = 43.679291, Region = "Asia" },
        new() { Code = "IE", Name = "Ireland", Latitude = 53.41291, Longitude = -8.24389, Region = "Europe" },
        new() { Code = "IL", Name = "Israel", Latitude = 31.046051, Longitude = 34.851612, Region = "Asia" },
        new() { Code = "IT", Name = "Italy", Latitude = 41.87194, Longitude = 12.56738, Region = "Europe" },
        new() { Code = "JM", Name = "Jamaica", Latitude = 18.109581, Longitude = -77.297508, Region = "Americas" },
        new() { Code = "JP", Name = "Japan", Latitude = 36.204824, Longitude = 138.252924, Region = "Asia" },
        new() { Code = "JO", Name = "Jordan", Latitude = 30.585164, Longitude = 36.238414, Region = "Asia" },
        new() { Code = "KZ", Name = "Kazakhstan", Latitude = 48.019573, Longitude = 66.923684, Region = "Asia" },
        new() { Code = "KE", Name = "Kenya", Latitude = -0.023559, Longitude = 37.906193, Region = "Africa" },
        new() { Code = "KI", Name = "Kiribati", Latitude = -3.370417, Longitude = -168.734039, Region = "Oceania" },
        new() { Code = "KW", Name = "Kuwait", Latitude = 29.31166, Longitude = 47.481766, Region = "Asia" },
        new() { Code = "KG", Name = "Kyrgyzstan", Latitude = 41.20438, Longitude = 74.766098, Region = "Asia" },
        new() { Code = "LA", Name = "Laos", Latitude = 19.85627, Longitude = 102.495496, Region = "Asia" },
        new() { Code = "LV", Name = "Latvia", Latitude = 56.879635, Longitude = 24.603189, Region = "Europe" },
        new() { Code = "LB", Name = "Lebanon", Latitude = 33.854721, Longitude = 35.862285, Region = "Asia" },
        new() { Code = "LS", Name = "Lesotho", Latitude = -29.609988, Longitude = 28.233608, Region = "Africa" },
        new() { Code = "LR", Name = "Liberia", Latitude = 6.428055, Longitude = -9.429499, Region = "Africa" },
        new() { Code = "LY", Name = "Libya", Latitude = 26.3351, Longitude = 17.228331, Region = "Africa" },
        new() { Code = "LI", Name = "Liechtenstein", Latitude = 47.166, Longitude = 9.555373, Region = "Europe" },
        new() { Code = "LT", Name = "Lithuania", Latitude = 55.169438, Longitude = 23.881275, Region = "Europe" },
        new() { Code = "LU", Name = "Luxembourg", Latitude = 49.815273, Longitude = 6.129583, Region = "Europe" },
        new() { Code = "MG", Name = "Madagascar", Latitude = -18.766947, Longitude = 46.869107, Region = "Africa" },
        new() { Code = "MW", Name = "Malawi", Latitude = -13.254308, Longitude = 34.301525, Region = "Africa" },
        new() { Code = "MY", Name = "Malaysia", Latitude = 4.210484, Longitude = 101.975766, Region = "Asia" },
        new() { Code = "MV", Name = "Maldives", Latitude = 3.202778, Longitude = 73.22068, Region = "Asia" },
        new() { Code = "ML", Name = "Mali", Latitude = 17.570692, Longitude = -3.996166, Region = "Africa" },
        new() { Code = "MT", Name = "Malta", Latitude = 35.937496, Longitude = 14.375416, Region = "Europe" },
        new() { Code = "MH", Name = "Marshall Islands", Latitude = 7.131474, Longitude = 171.184478, Region = "Oceania" },
        new() { Code = "MR", Name = "Mauritania", Latitude = 21.00789, Longitude = -10.940835, Region = "Africa" },
        new() { Code = "MU", Name = "Mauritius", Latitude = -20.348404, Longitude = 57.552152, Region = "Africa" },
        new() { Code = "MX", Name = "Mexico", Latitude = 23.634501, Longitude = -102.552784, Region = "Americas" },
        new() { Code = "FM", Name = "Micronesia", Latitude = 7.425554, Longitude = 150.550812, Region = "Oceania" },
        new() { Code = "MD", Name = "Moldova", Latitude = 47.411631, Longitude = 28.369885, Region = "Europe" },
        new() { Code = "MC", Name = "Monaco", Latitude = 43.750298, Longitude = 7.412841, Region = "Europe" },
        new() { Code = "MN", Name = "Mongolia", Latitude = 46.862496, Longitude = 103.846656, Region = "Asia" },
        new() { Code = "ME", Name = "Montenegro", Latitude = 42.708678, Longitude = 19.37439, Region = "Europe" },
        new() { Code = "MA", Name = "Morocco", Latitude = 31.791702, Longitude = -7.09262, Region = "Africa" },
        new() { Code = "MZ", Name = "Mozambique", Latitude = -18.665695, Longitude = 35.529562, Region = "Africa" },
        new() { Code = "MM", Name = "Myanmar", Latitude = 21.913965, Longitude = 95.956223, Region = "Asia" },
        new() { Code = "NA", Name = "Namibia", Latitude = -22.95764, Longitude = 18.49041, Region = "Africa" },
        new() { Code = "NR", Name = "Nauru", Latitude = -0.522778, Longitude = 166.931503, Region = "Oceania" },
        new() { Code = "NP", Name = "Nepal", Latitude = 28.394857, Longitude = 84.124008, Region = "Asia" },
        new() { Code = "NL", Name = "Netherlands", Latitude = 52.132633, Longitude = 5.291266, Region = "Europe" },
        new() { Code = "NZ", Name = "New Zealand", Latitude = -40.900557, Longitude = 174.885971, Region = "Oceania" },
        new() { Code = "NI", Name = "Nicaragua", Latitude = 12.865416, Longitude = -85.207229, Region = "Americas" },
        new() { Code = "NE", Name = "Niger", Latitude = 17.607789, Longitude = 8.081666, Region = "Africa" },
        new() { Code = "NG", Name = "Nigeria", Latitude = 9.081999, Longitude = 8.675277, Region = "Africa" },
        new() { Code = "KP", Name = "North Korea", Latitude = 40.339852, Longitude = 127.510093, Region = "Asia" },
        new() { Code = "MK", Name = "North Macedonia", Latitude = 41.608635, Longitude = 21.745275, Region = "Europe" },
        new() { Code = "NO", Name = "Norway", Latitude = 60.472024, Longitude = 8.468946, Region = "Europe" },
        new() { Code = "OM", Name = "Oman", Latitude = 21.512583, Longitude = 55.923255, Region = "Asia" },
        new() { Code = "PK", Name = "Pakistan", Latitude = 30.375321, Longitude = 69.345116, Region = "Asia" },
        new() { Code = "PW", Name = "Palau", Latitude = 7.51498, Longitude = 134.58252, Region = "Oceania" },
        new() { Code = "PS", Name = "Palestine", Latitude = 31.947352, Longitude = 35.227153, Region = "Asia" },
        new() { Code = "PA", Name = "Panama", Latitude = 8.537981, Longitude = -80.782127, Region = "Americas" },
        new() { Code = "PG", Name = "Papua New Guinea", Latitude = -6.314993, Longitude = 143.95555, Region = "Oceania" },
        new() { Code = "PY", Name = "Paraguay", Latitude = -23.442503, Longitude = -58.443832, Region = "Americas" },
        new() { Code = "PE", Name = "Peru", Latitude = -9.189967, Longitude = -75.015152, Region = "Americas" },
        new() { Code = "PH", Name = "Philippines", Latitude = 12.879721, Longitude = 121.774017, Region = "Asia" },
        new() { Code = "PL", Name = "Poland", Latitude = 51.919438, Longitude = 19.145136, Region = "Europe" },
        new() { Code = "PT", Name = "Portugal", Latitude = 39.399872, Longitude = -8.224454, Region = "Europe" },
        new() { Code = "QA", Name = "Qatar", Latitude = 25.354826, Longitude = 51.183884, Region = "Asia" },
        new() { Code = "RO", Name = "Romania", Latitude = 45.943161, Longitude = 24.96676, Region = "Europe" },
        new() { Code = "RU", Name = "Russia", Latitude = 61.52401, Longitude = 105.318756, Region = "Europe" },
        new() { Code = "RW", Name = "Rwanda", Latitude = -1.940278, Longitude = 29.873888, Region = "Africa" },
        new() { Code = "KN", Name = "Saint Kitts and Nevis", Latitude = 17.357822, Longitude = -62.782998, Region = "Americas" },
        new() { Code = "LC", Name = "Saint Lucia", Latitude = 13.909444, Longitude = -60.978893, Region = "Americas" },
        new() { Code = "VC", Name = "Saint Vincent and the Grenadines", Latitude = 12.984305, Longitude = -61.287228, Region = "Americas" },
        new() { Code = "WS", Name = "Samoa", Latitude = -13.759029, Longitude = -172.104629, Region = "Oceania" },
        new() { Code = "SM", Name = "San Marino", Latitude = 43.94236, Longitude = 12.457777, Region = "Europe" },
        new() { Code = "ST", Name = "Sao Tome and Principe", Latitude = 0.18636, Longitude = 6.613081, Region = "Africa" },
        new() { Code = "SA", Name = "Saudi Arabia", Latitude = 23.885942, Longitude = 45.079162, Region = "Asia" },
        new() { Code = "SN", Name = "Senegal", Latitude = 14.497401, Longitude = -14.452362, Region = "Africa" },
        new() { Code = "RS", Name = "Serbia", Latitude = 44.016521, Longitude = 21.005859, Region = "Europe" },
        new() { Code = "SC", Name = "Seychelles", Latitude = -4.679574, Longitude = 55.491977, Region = "Africa" },
        new() { Code = "SL", Name = "Sierra Leone", Latitude = 8.460555, Longitude = -11.779889, Region = "Africa" },
        new() { Code = "SG", Name = "Singapore", Latitude = 1.352083, Longitude = 103.819836, Region = "Asia" },
        new() { Code = "SK", Name = "Slovakia", Latitude = 48.669026, Longitude = 19.699024, Region = "Europe" },
        new() { Code = "SI", Name = "Slovenia", Latitude = 46.151241, Longitude = 14.995463, Region = "Europe" },
        new() { Code = "SB", Name = "Solomon Islands", Latitude = -9.64571, Longitude = 160.156194, Region = "Oceania" },
        new() { Code = "SO", Name = "Somalia", Latitude = 5.152149, Longitude = 46.199616, Region = "Africa" },
        new() { Code = "ZA", Name = "South Africa", Latitude = -30.559482, Longitude = 22.937506, Region = "Africa" },
        new() { Code = "KR", Name = "South Korea", Latitude = 35.907757, Longitude = 127.766922, Region = "Asia" },
        new() { Code = "SS", Name = "South Sudan", Latitude = 7.8626845, Longitude = 30.217636, Region = "Africa" },
        new() { Code = "ES", Name = "Spain", Latitude = 40.463667, Longitude = -3.74922, Region = "Europe" },
        new() { Code = "LK", Name = "Sri Lanka", Latitude = 7.873054, Longitude = 80.771797, Region = "Asia" },
        new() { Code = "SD", Name = "Sudan", Latitude = 12.862807, Longitude = 30.217636, Region = "Africa" },
        new() { Code = "SR", Name = "Suriname", Latitude = 3.919305, Longitude = -56.027783, Region = "Americas" },
        new() { Code = "SE", Name = "Sweden", Latitude = 60.128161, Longitude = 18.643501, Region = "Europe" },
        new() { Code = "CH", Name = "Switzerland", Latitude = 46.818188, Longitude = 8.227512, Region = "Europe" },
        new() { Code = "SY", Name = "Syria", Latitude = 34.802075, Longitude = 38.996815, Region = "Asia" },
        new() { Code = "TW", Name = "Taiwan", Latitude = 23.69781, Longitude = 120.960515, Region = "Asia" },
        new() { Code = "TJ", Name = "Tajikistan", Latitude = 38.861034, Longitude = 71.276093, Region = "Asia" },
        new() { Code = "TZ", Name = "Tanzania", Latitude = -6.369028, Longitude = 34.888822, Region = "Africa" },
        new() { Code = "TH", Name = "Thailand", Latitude = 15.870032, Longitude = 100.992541, Region = "Asia" },
        new() { Code = "TL", Name = "Timor-Leste", Latitude = -8.874217, Longitude = 125.727539, Region = "Asia" },
        new() { Code = "TG", Name = "Togo", Latitude = 8.619543, Longitude = 0.824782, Region = "Africa" },
        new() { Code = "TO", Name = "Tonga", Latitude = -21.178986, Longitude = -175.198242, Region = "Oceania" },
        new() { Code = "TT", Name = "Trinidad and Tobago", Latitude = 10.691803, Longitude = -61.222503, Region = "Americas" },
        new() { Code = "TN", Name = "Tunisia", Latitude = 33.886917, Longitude = 9.537499, Region = "Africa" },
        new() { Code = "TR", Name = "Turkey", Latitude = 38.963745, Longitude = 35.243322, Region = "Asia" },
        new() { Code = "TM", Name = "Turkmenistan", Latitude = 38.969719, Longitude = 59.556278, Region = "Asia" },
        new() { Code = "TV", Name = "Tuvalu", Latitude = -7.109535, Longitude = 177.64933, Region = "Oceania" },
        new() { Code = "UG", Name = "Uganda", Latitude = 1.373333, Longitude = 32.290275, Region = "Africa" },
        new() { Code = "UA", Name = "Ukraine", Latitude = 48.379433, Longitude = 31.16558, Region = "Europe" },
        new() { Code = "AE", Name = "United Arab Emirates", Latitude = 23.424076, Longitude = 53.847818, Region = "Asia" },
        new() { Code = "GB", Name = "United Kingdom", Latitude = 55.378051, Longitude = -3.435973, Region = "Europe" },
        new() { Code = "US", Name = "United States", Latitude = 37.09024, Longitude = -95.712891, Region = "Americas" },
        new() { Code = "UY", Name = "Uruguay", Latitude = -32.522779, Longitude = -55.765835, Region = "Americas" },
        new() { Code = "UZ", Name = "Uzbekistan", Latitude = 41.377491, Longitude = 64.585262, Region = "Asia" },
        new() { Code = "VU", Name = "Vanuatu", Latitude = -15.376706, Longitude = 166.959158, Region = "Oceania" },
        new() { Code = "VA", Name = "Vatican City", Latitude = 41.902916, Longitude = 12.453389, Region = "Europe" },
        new() { Code = "VE", Name = "Venezuela", Latitude = 6.42375, Longitude = -66.58973, Region = "Americas" },
        new() { Code = "VN", Name = "Vietnam", Latitude = 14.058324, Longitude = 108.277199, Region = "Asia" },
        new() { Code = "YE", Name = "Yemen", Latitude = 15.552727, Longitude = 48.516388, Region = "Asia" },
        new() { Code = "ZM", Name = "Zambia", Latitude = -13.133897, Longitude = 27.849332, Region = "Africa" },
        new() { Code = "ZW", Name = "Zimbabwe", Latitude = -19.015438, Longitude = 29.154857, Region = "Africa" },
    };

    private record IpApiResponse(
        [property: JsonPropertyName("status")] string? Status,
        [property: JsonPropertyName("countryCode")] string? CountryCode
    );
}
