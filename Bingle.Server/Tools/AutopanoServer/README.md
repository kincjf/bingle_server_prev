AutoPanoServer Installation
---
1. �ֽ� Linux OS�� �̿��� �� (glibc not found error�� �߻� �� ���� ����)
2. ������ system path ����
(���� ��� ���� : http://stackoverflow.com/questions/480764/linux-error-while-loading-shared-libraries-cannot-open-shared-object-file-no-s)

> 1. Check for the existence of the dynamic library path environnement variable(LD_LIBRARY_PATH)
> 2. if there is nothing to be display we need to add the default path value (or not as you wich)
> echo $LD_LIBRARY_PATH
> 3. We add the desire path and export it and try the application
> LD_LIBRARY_PATH=/usr/local/lib; LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/my_library/path.so.something  
> 4. export LD_LIBRARY_PATH
> 5 ./AutopanoServer -v
> (reference : http://www.gnu.org/software/gsl/manual/html_node/Shared-Libraries.html)

> * ���� �����ϰ� ������ ". AutopanoServer.sh" �Է�, "./AutopanoServer -v" Ȯ��