#pragma once

class Config
{
private:
    string _logFileName;
    string _rabbitUserName;
    string _rabbitPassword;
    string _rabbitVHost;
    string _rabbitHost;
    int _rabbitPort;

public:
    Config(const string fileName);

    const string GetLogFileName()
    {
        return _logFileName;
    }

    void SetLogFileName(string logFileName)
    {
        _logFileName = logFileName;
    }

    const string RabbitUserName() 
    {
        return _rabbitUserName;
    }

    string RabbitPassword{ get; set; }
    string RabbitVHost{ get; set; }
    string RabbitHost{ get; set; }
    int RabbitPort{ get; set; }
*/
};
